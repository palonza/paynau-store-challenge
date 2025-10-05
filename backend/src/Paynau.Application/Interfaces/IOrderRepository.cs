
namespace Paynau.Application.Interfaces;

using Paynau.Domain.Entities;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task<bool> HasOrdersForProductAsync(int productId);
    Task SaveChangesAsync();
}

