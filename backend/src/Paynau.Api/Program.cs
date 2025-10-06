using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Paynau.Api.Extensions;
using Paynau.Api.Middleware;
using Paynau.Application.Interfaces;
using Paynau.Domain.Interfaces;
using Paynau.Domain.Services;
using Paynau.Infrastructure.Data;
using Paynau.Infrastructure.Logging;
using Paynau.Infrastructure.Repositories;
using Paynau.Infrastructure.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = LoggingExtensions.CreateLogger(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Paynau API", Version = "v1" });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<PaynauDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// CQRS and MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Paynau.Application.Commands.CreateProductCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Paynau.Application.Commands.CreateProductCommand).Assembly);

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Domain Services
builder.Services.AddScoped<IOrderDomainService, OrderDomainService>();

// Seeder
builder.Services.AddScoped<DataSeeder>();

// JWT Configuration
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed data in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (args.Contains("--migrate"))
{
    Console.WriteLine("🧩 Running migrations on startup...");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PaynauDbContext>();
    db.Database.Migrate();
    Console.WriteLine("✅ Migrations applied successfully.");
    return;
}

app.Run();

