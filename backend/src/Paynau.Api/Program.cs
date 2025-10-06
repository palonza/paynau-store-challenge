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

// Usar versión fija en lugar de AutoDetect para evitar problemas de conexión durante el inicio
builder.Services.AddDbContext<PaynauDbContext>(options =>
    options.UseMySql(
        connectionString, 
        new MySqlServerVersion(new Version(8, 0, 30))
    ));

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

// Aplicar migraciones y seeding al inicio
Console.WriteLine("Initializing database...");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaynauDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Verificar conexión
        logger.LogInformation("Testing database connection...");
        var canConnect = await db.Database.CanConnectAsync();
        if (!canConnect)
        {
            logger.LogError("Cannot connect to database");
            throw new Exception("Database connection failed");
        }
        
        logger.LogInformation("Database connection successful");
        
        // Mostrar connection string (sin password)
        var connString = builder.Configuration.GetConnectionString("Default");
        var safeConnString = System.Text.RegularExpressions.Regex.Replace(connString ?? "", @"Password=[^;]*", "Password=***");
        logger.LogInformation("Connection: {ConnectionString}", safeConnString);
        
        // Ver migraciones pendientes
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        var appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
        
        logger.LogInformation("Applied migrations: {Count}", appliedMigrations.Count());
        foreach (var migration in appliedMigrations)
        {
            logger.LogInformation("  - {Migration}", migration);
        }
        
        logger.LogInformation("Pending migrations: {Count}", pendingMigrations.Count());
        foreach (var migration in pendingMigrations)
        {
            logger.LogInformation("  - {Migration}", migration);
        }
        
        // Aplicar migraciones
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
            await db.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is already up to date");
        }
        
        // Verificar que las tablas existan
        try
        {
            var productsExist = await db.Products.AnyAsync();
            logger.LogInformation("Products table exists: {Exists}", true);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Could not verify Products table: {Message}", ex.Message);
        }
        
        // Seed data en Development
        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Starting data seeding...");
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database initialization: {Message}", ex.Message);
        throw;
    }
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

app.Run();