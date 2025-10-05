
namespace Paynau.Application.Interfaces;

using Paynau.Domain.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task SaveChangesAsync();
}

