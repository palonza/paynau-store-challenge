
namespace Paynau.Tests.Domain;

using FluentAssertions;
using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;
using Paynau.Domain.Services;
using Xunit;

public class OrderDomainServiceTests
{
    private readonly OrderDomainService _service;

    public OrderDomainServiceTests()
    {
        _service = new OrderDomainService();
    }

    [Fact]
    public void CreateOrder_WithSufficientStock_ShouldCreateOrderAndDecreaseStock()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-end laptop", 1500, 10);
        var initialStock = product.Stock;

        // Act
        var order = _service.CreateOrder(product, 3);

        // Assert
        order.Should().NotBeNull();
        order.ProductId.Should().Be(product.Id);
        order.Quantity.Should().Be(3);
        order.Total.Should().Be(1500 * 3);
        product.Stock.Should().Be(initialStock - 3);
    }

    [Fact]
    public void CreateOrder_WithInsufficientStock_ShouldThrowInsufficientStockException()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-end laptop", 1500, 5);

        // Act
        var act = () => _service.CreateOrder(product, 10);

        // Assert
        act.Should().Throw<InsufficientStockException>();
    }

    [Fact]
    public void CreateOrder_WithNullProduct_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _service.CreateOrder(null!, 5);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

