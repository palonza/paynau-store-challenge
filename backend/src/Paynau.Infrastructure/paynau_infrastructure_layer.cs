// ========================================
// Paynau.Infrastructure.csproj
// ========================================
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.8" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Paynau.Domain\Paynau.Domain.csproj" />
    <ProjectReference Include="..\Paynau.Application\Paynau.Application.csproj" />
  </ItemGroup>

  <ItemGroup>
    <None Update="Seed\products.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
    <None Update="Seed\orders.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>

// ========================================
// Data/PaynauDbContext.cs
// ========================================
namespace Paynau.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Paynau.Domain.Entities;

public class PaynauDbContext : DbContext
{
    public PaynauDbContext(DbContextOptions<PaynauDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaynauDbContext).Assembly);
    }
}

// ========================================
// Data/Configurations/ProductConfiguration.cs
// ========================================
namespace Paynau.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paynau.Domain.Entities;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(p => p.Name)
            .IsUnique();
    }
}

// ========================================
// Data/Configurations/OrderConfiguration.cs
// ========================================
namespace Paynau.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paynau.Domain.Entities;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.ProductId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ========================================
// Repositories/ProductRepository.cs
// ========================================
namespace Paynau.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Paynau.Application.Interfaces;
using Paynau.Domain.Entities;
using Paynau.Infrastructure.Data;

public class ProductRepository : IProductRepository
{
    private readonly PaynauDbContext _context;

    public ProductRepository(PaynauDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.OrderBy(p => p.Id).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        return product;
    }

    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Name == name);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

// ========================================
// Repositories/OrderRepository.cs
// ========================================
namespace Paynau.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Paynau.Application.Interfaces;
using Paynau.Domain.Entities;
using Paynau.Infrastructure.Data;

public class OrderRepository : IOrderRepository
{
    private readonly PaynauDbContext _context;

    public OrderRepository(PaynauDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        return order;
    }

    public async Task<bool> HasOrdersForProductAsync(int productId)
    {
        return await _context.Orders.AnyAsync(o => o.ProductId == productId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

// ========================================
// Seed/DataSeeder.cs
// ========================================
namespace Paynau.Infrastructure.Seed;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paynau.Domain.Entities;
using Paynau.Infrastructure.Data;
using System.Text.Json;

public class DataSeeder
{
    private readonly PaynauDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(PaynauDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();

            if (!await _context.Products.AnyAsync())
            {
                await SeedProductsAsync();
            }

            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedProductsAsync()
    {
        var productsJson = await File.ReadAllTextAsync("Seed/products.json");
        var productData = JsonSerializer.Deserialize<List<ProductSeedDto>>(productsJson);

        if (productData == null || !productData.Any())
        {
            _logger.LogWarning("No products found in seed file");
            return;
        }

        var products = new List<Product>();

        foreach (var dto in productData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || 
                    string.IsNullOrWhiteSpace(dto.Description) || 
                    dto.Price < 0 || 
                    dto.Stock < 0)
                {
                    _logger.LogWarning("Skipping invalid product: {ProductId}", dto.Id);
                    continue;
                }

                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name == dto.Name);

                if (existingProduct != null)
                {
                    _logger.LogWarning("Product with name '{ProductName}' already exists. Skipping.", dto.Name);
                    continue;
                }

                var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.Stock);
                products.Add(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductId}", dto.Id);
            }
        }

        if (products.Any())
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} products", products.Count);
        }
    }

    private record ProductSeedDto(int Id, string Name, string Description, decimal Price, int Stock);
}

// ========================================
// Logging/LoggingExtensions.cs
// ========================================
namespace Paynau.Infrastructure.Logging;

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Http;

public static class LoggingExtensions
{
    public static ILogger CreateLogger(IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Paynau")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        var logtailEnabled = configuration.GetValue<bool>("LOGTAIL:ENABLED");
        var logtailToken = configuration.GetValue<string>("LOGTAIL:SOURCE_TOKEN");
        var logtailEndpoint = configuration.GetValue<string>("LOGTAIL:ENDPOINT") ?? "https://in.logtail.com";

        if (logtailEnabled && !string.IsNullOrWhiteSpace(logtailToken))
        {
            loggerConfig.WriteTo.Http(
                requestUri: logtailEndpoint,
                queueLimitBytes: null,
                httpClient: new LogtailHttpClient(logtailToken));
        }

        return loggerConfig.CreateLogger();
    }

    private class LogtailHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public LogtailHttpClient(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _httpClient.PostAsync(requestUri, content);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
