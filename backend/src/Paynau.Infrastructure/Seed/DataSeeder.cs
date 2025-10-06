
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

            if (_context.Database.IsRelational())
            {
                _logger.LogInformation("Clearing existing data and resetting identity counters (Development only)...");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Orders;");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Products;");

                // Reiniciar contadores AUTO_INCREMENT
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders AUTO_INCREMENT = 1;");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Products AUTO_INCREMENT = 1;");


                _logger.LogInformation("✅ Data cleared and identity counters reset successfully (Development).");
            }

            if (!await _context.Products.AnyAsync())
            {
                await SeedProductsAsync();
            }

            if (!await _context.Orders.AnyAsync())
            {
                await SeedOrdersAsync();
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
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
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

    private async Task SeedOrdersAsync()
    {
        var ordersJson = await File.ReadAllTextAsync("Seed/orders.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var orderData = JsonSerializer.Deserialize<List<OrderSeedDto>>(ordersJson);

        if (orderData == null || !orderData.Any())
        {
            _logger.LogWarning("No orders found in seed file");
            return;
        }

        var orders = new List<Order>();

        foreach (var dto in orderData)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Skipping order {OrderId}: product {ProductId} not found", dto.Id, dto.ProductId);
                    continue;
                }

                if (dto.Quantity <= 0)
                {
                    _logger.LogWarning("Skipping order {OrderId}: invalid quantity {Quantity}", dto.Id, dto.Quantity);
                    continue;
                }

                if (product.Stock < dto.Quantity)
                {
                    _logger.LogWarning("Skipping order {OrderId}: insufficient stock for product {ProductId}", dto.Id, dto.ProductId);
                    continue;
                }

                var total = product.Price * dto.Quantity;

                // var order = new Order
                // {
                //     ProductId = product.Id,
                //     Quantity = dto.Quantity,
                //     Total = total,
                //     CreatedAt = DateTime.UtcNow
                // };

                // orders.Add(order);

                // // Descontar stock del producto
                // product.Stock -= dto.Quantity;

                // eliminamos la mutacion de datos directa y la reemplazamos mediante el metodo de la fabrica del dominio y luego actualizamos el stock
                var order = Order.Create(product.Id, dto.Quantity, total);
                orders.Add(order);
                product.DecreaseStock(dto.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order {OrderId}", dto.Id);
            }
        }

        if (orders.Any())
        {
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} orders", orders.Count);
        }
    }

    private record OrderSeedDto(int Id, int ProductId, int Quantity);
    private record ProductSeedDto(int Id, string Name, string Description, decimal Price, int Stock);
}

