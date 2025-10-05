using Paynau.Domain.Exceptions;

namespace Paynau.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    private Product() { }

    public static Product Create(string name, string description, decimal price, int stock)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateStock(stock);

        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock
        };
    }

    public void Update(string name, string description, decimal price, int stock)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateStock(stock);

        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidQuantityException("Quantity must be greater than zero");

        if (Stock < quantity)
            throw new InsufficientStockException($"Insufficient stock for product {Name}. Available: {Stock}, Requested: {quantity}");

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidQuantityException("Quantity must be greater than zero");

        Stock += quantity;
    }

    public bool HasSufficientStock(int quantity) => Stock >= quantity;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description is required");
    }

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Product price cannot be negative");
    }

    private static void ValidateStock(int stock)
    {
        if (stock < 0)
            throw new DomainException("Product stock cannot be negative");
    }
}

