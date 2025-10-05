// ========================================
// Paynau.Api.csproj
// ========================================
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.8">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.7.3" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.2" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Sinks.Http" Version="9.0.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Paynau.Domain\Paynau.Domain.csproj" />
    <ProjectReference Include="..\Paynau.Application\Paynau.Application.csproj" />
    <ProjectReference Include="..\Paynau.Infrastructure\Paynau.Infrastructure.csproj" />
  </ItemGroup>
</Project>

// ========================================
// Program.cs
// ========================================
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

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// ========================================
// Controllers/ProductsController.cs
// ========================================
namespace Paynau.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Queries;
using Paynau.Domain.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        _logger.LogInformation("Getting all products");
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound(new { message = $"Product with ID {id} was not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        _logger.LogInformation("Creating product {ProductName}", dto.Name);
        
        var command = new CreateProductCommand(dto.Name, dto.Description, dto.Price, dto.Stock);
        var product = await _mediator.Send(command);
        
        _logger.LogInformation("Product {ProductId} created successfully", product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        _logger.LogInformation("Updating product {ProductId}", id);
        
        var command = new UpdateProductCommand(id, dto.Name, dto.Description, dto.Price, dto.Stock);
        var product = await _mediator.Send(command);
        
        _logger.LogInformation("Product {ProductId} updated successfully", id);
        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);
        
        await _mediator.Send(new DeleteProductCommand(id));
        
        _logger.LogInformation("Product {ProductId} deleted successfully", id);
        return NoContent();
    }
}

// ========================================
// Controllers/OrdersController.cs
// ========================================
namespace Paynau.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Queries;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        _logger.LogInformation("Getting all orders");
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        
        if (!orders.Any())
        {
            _logger.LogInformation("No orders found");
            return NoContent();
        }

        return Ok(orders);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating order for product {ProductId}", dto.ProductId);
        
        var command = new CreateOrderCommand(dto.ProductId, dto.Quantity);
        var order = await _mediator.Send(command);
        
        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
    }
}

// ========================================
// Middleware/ExceptionHandlingMiddleware.cs
// ========================================
namespace Paynau.Api.Middleware;

using Paynau.Domain.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ProductNotFoundException => HttpStatusCode.NotFound,
            InsufficientStockException => HttpStatusCode.BadRequest,
            InvalidQuantityException => HttpStatusCode.BadRequest,
            DuplicateProductException => HttpStatusCode.Conflict,
            ProductInUseException => HttpStatusCode.Conflict,
            ConcurrencyException => HttpStatusCode.Conflict,
            DomainException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = exception.Message,
            statusCode = (int)statusCode,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

// ========================================
// Extensions/ServiceCollectionExtensions.cs
// ========================================
namespace Paynau.Api.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtIssuer = configuration["JWT:Issuer"] ?? "paynau-dev";
        var jwtAudience = configuration["JWT:Audience"] ?? "paynau-api";
        var jwtKey = configuration["JWT:Key"] ?? "supersecretdevkey12345678901234567890";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        return services;
    }
}

// ========================================
// Extensions/JwtExtensions.cs
// ========================================
namespace Paynau.Api.Extensions;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class JwtExtensions
{
    public static string GenerateJwtToken(string issuer, string audience, string key)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "paynau-user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "admin")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// ========================================
// appsettings.json
// ========================================
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=PaynauDb;User=root;Password=pass;"
  },
  "JWT": {
    "Issuer": "paynau-dev",
    "Audience": "paynau-api",
    "Key": "supersecretdevkey12345678901234567890"
  },
  "LOGTAIL": {
    "ENABLED": false,
    "SOURCE_TOKEN": "",
    "ENDPOINT": "https://in.logtail.com"
  }
}

// ========================================
// appsettings.Development.json
// ========================================
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "Default": "Server=mysql;Database=PaynauDb;User=root;Password=MySqlP@ssw0rd;"
  }
}
