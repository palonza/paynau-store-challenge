using System.Text.Json.Serialization;

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
            _logger.LogInformation("Starting database seeding process...");

            // 🔧 FIX: Asegurar que las migraciones estén aplicadas
            _logger.LogInformation("Ensuring database migrations are applied...");
            await _context.Database.MigrateAsync();
            _logger.LogInformation("✅ Database migrations are up to date");

            // 🔧 FIX: Verificar existencia de tablas de manera segura con SQL
            var tablesExist = await CheckTablesExistSafeAsync();
            
            if (!tablesExist)
            {
                _logger.LogWarning("Tables don't exist yet. Skipping data seeding. This might indicate a migration issue.");
                return;
            }

            _logger.LogInformation("✅ Database tables exist and are ready");

            // 🔧 OPCIONAL: Solo limpiar datos si realmente queremos resetear
            // En producción, esto debería estar deshabilitado
            // await ClearExistingDataAsync();

            // Seed products
            var productsExist = await _context.Products.AnyAsync();
            if (!productsExist)
            {
                _logger.LogInformation("Products table is empty. Starting product seeding...");
                await SeedProductsAsync();
            }
            else
            {
                _logger.LogInformation("Products already exist in database. Skipping product seeding.");
            }

            // Seed orders
            var ordersExist = await _context.Orders.AnyAsync();
            if (!ordersExist)
            {
                _logger.LogInformation("Orders table is empty. Starting order seeding...");
                await SeedOrdersAsync();
            }
            else
            {
                _logger.LogInformation("Orders already exist in database. Skipping order seeding.");
            }

            _logger.LogInformation("✅ Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ An error occurred while seeding the database: {Message}", ex.Message);
            // NO lanzar la excepción para evitar que la app crashee y entre en loop
            // En su lugar, solo loggear el error
        }
    }

    private async Task<bool> CheckTablesExistSafeAsync()
    {
        try
        {
            if (!_context.Database.IsRelational())
            {
                return true; // Para bases de datos en memoria
            }

            // Verificar usando una query SQL directa que es más segura
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_schema = DATABASE() 
                AND table_name IN ('Products', 'Orders')";

            var result = await command.ExecuteScalarAsync();
            await connection.CloseAsync();

            var tableCount = Convert.ToInt32(result);
            _logger.LogInformation("Found {TableCount} tables in database", tableCount);

            return tableCount == 2; // Ambas tablas deben existir
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tables exist: {Message}", ex.Message);
            return false;
        }
    }

    private async Task ClearExistingDataAsync()
    {
        try
        {
            _logger.LogInformation("Clearing existing data (Development mode)...");

            // Verificar si hay datos antes de intentar borrar
            var hasOrders = await _context.Orders.AnyAsync();
            var hasProducts = await _context.Products.AnyAsync();

            if (hasOrders)
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Orders;");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Orders AUTO_INCREMENT = 1;");
                _logger.LogInformation("Cleared Orders table");
            }

            if (hasProducts)
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Products;");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Products AUTO_INCREMENT = 1;");
                _logger.LogInformation("Cleared Products table");
            }

            _logger.LogInformation("✅ Data cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not clear existing data: {Message}", ex.Message);
        }
    }

    private async Task SeedProductsAsync()
    {
        try
        {
            var seedPath = Path.Combine(AppContext.BaseDirectory, "Seed", "products.json");
            
            _logger.LogInformation("Looking for products seed file at: {SeedPath}", seedPath);
            
            if (!File.Exists(seedPath))
            {
                _logger.LogError("❌ Products seed file not found at: {SeedPath}", seedPath);
                return;
            }

            var productsJson = await File.ReadAllTextAsync(seedPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var productData = JsonSerializer.Deserialize<List<ProductSeedDto>>(productsJson, options);

            if (productData == null || !productData.Any())
            {
                _logger.LogWarning("⚠️ No products found in seed file");
                return;
            }

            _logger.LogInformation("Found {Count} products in seed file", productData.Count);

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
                        _logger.LogWarning("⚠️ Skipping invalid product: {ProductName}", dto.Name);
                        continue;
                    }

                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name == dto.Name);

                    if (existingProduct != null)
                    {
                        _logger.LogWarning("⚠️ Product '{ProductName}' already exists. Skipping.", dto.Name);
                        continue;
                    }

                    var product = Product.Create(dto.Name, dto.Description, dto.Price, dto.Stock);
                    products.Add(product);
                    _logger.LogDebug("Prepared product: {ProductName}", dto.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error creating product: {ProductName}", dto.Name);
                }
            }

            if (products.Any())
            {
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Successfully seeded {Count} products", products.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ No products were added to the database");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in SeedProductsAsync: {Message}", ex.Message);
        }
    }

    private async Task SeedOrdersAsync()
    {
        try
        {
            var seedPath = Path.Combine(AppContext.BaseDirectory, "Seed", "orders.json");
            
            _logger.LogInformation("Looking for orders seed file at: {SeedPath}", seedPath);
            
            if (!File.Exists(seedPath))
            {
                _logger.LogError("❌ Orders seed file not found at: {SeedPath}", seedPath);
                return;
            }

            var ordersJson = await File.ReadAllTextAsync(seedPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var orderData = JsonSerializer.Deserialize<List<OrderSeedDto>>(ordersJson, options);

            if (orderData == null || !orderData.Any())
            {
                _logger.LogWarning("⚠️ No orders found in seed file");
                return;
            }

            _logger.LogInformation("Found {Count} orders in seed file", orderData.Count);

            var orders = new List<Order>();

            foreach (var dto in orderData)
            {
                try
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("⚠️ Skipping order {OrderId}: product {ProductId} not found", dto.Id, dto.ProductId);
                        continue;
                    }

                    if (dto.Quantity <= 0)
                    {
                        _logger.LogWarning("⚠️ Skipping order {OrderId}: invalid quantity {Quantity}", dto.Id, dto.Quantity);
                        continue;
                    }

                    if (product.Stock < dto.Quantity)
                    {
                        _logger.LogWarning("⚠️ Skipping order {OrderId}: insufficient stock for product {ProductId} (Available: {Stock}, Requested: {Quantity})", 
                            dto.Id, dto.ProductId, product.Stock, dto.Quantity);
                        continue;
                    }

                    var order = Order.Create(product.Id, dto.Quantity, product.Price);
                    orders.Add(order);
                    product.DecreaseStock(dto.Quantity);
                    _logger.LogDebug("Prepared order for product {ProductId}, quantity {Quantity}", dto.ProductId, dto.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error creating order {OrderId}", dto.Id);
                }
            }

            if (orders.Any())
            {
                await _context.Orders.AddRangeAsync(orders);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Successfully seeded {Count} orders", orders.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ No orders were added to the database");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in SeedOrdersAsync: {Message}", ex.Message);
        }
    }

    // private record OrderSeedDto(int Id, int ProductId, int Quantity);
    // private record ProductSeedDto(int Id, string Name, string Description, decimal Price, int Stock);
}

public class OrderSeedDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("productId")] public int ProductId { get; set; }
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
}

public class ProductSeedDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("stock")] public int Stock { get; set; }
}