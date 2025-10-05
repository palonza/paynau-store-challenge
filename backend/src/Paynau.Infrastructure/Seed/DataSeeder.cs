
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

