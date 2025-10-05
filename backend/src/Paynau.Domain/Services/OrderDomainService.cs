
namespace Paynau.Domain.Services;

using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;
using Paynau.Domain.Interfaces;

public class OrderDomainService : IOrderDomainService
{
    public Order CreateOrder(Product product, int quantity)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (!product.HasSufficientStock(quantity))
            throw new InsufficientStockException(
                $"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {quantity}");

        var order = Order.Create(product.Id, quantity, product.Price);
        product.DecreaseStock(quantity);

        return order;
    }
}
