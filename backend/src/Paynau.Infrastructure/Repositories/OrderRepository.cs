
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

