using Paynau.Domain.Exceptions;

namespace Paynau.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Product? Product { get; private set; }

    private Order() { }

    public static Order Create(int productId, int quantity, decimal productPrice)
    {
        if (quantity <= 0)
            throw new InvalidQuantityException("Order quantity must be greater than zero");

        if (productPrice < 0)
            throw new DomainException("Product price cannot be negative");

        return new Order
        {
            ProductId = productId,
            Quantity = quantity,
            Total = productPrice * quantity,
            CreatedAt = DateTime.UtcNow
        };
    }
}

