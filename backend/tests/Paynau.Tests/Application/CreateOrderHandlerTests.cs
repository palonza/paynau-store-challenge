
namespace Paynau.Tests.Application;

using FluentAssertions;
using Moq;
using Paynau.Application.Commands;
using Paynau.Application.Handlers;
using Paynau.Application.Interfaces;
using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;
using Paynau.Domain.Interfaces;
using Xunit;

public class CreateOrderHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IOrderDomainService> _mockDomainService;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockDomainService = new Mock<IOrderDomainService>();
        
        _handler = new CreateOrderHandler(
            _mockProductRepository.Object,
            _mockOrderRepository.Object,
            _mockDomainService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrder()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-end laptop", 1500, 10);
        var order = Order.Create(product.Id, 3, product.Price);
        var command = new CreateOrderCommand(product.Id, 3);

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(command.ProductId))
            .ReturnsAsync(product);

        _mockDomainService
            .Setup(x => x.CreateOrder(product, command.Quantity))
            .Returns(order);

        _mockOrderRepository
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(product.Id);
        result.Quantity.Should().Be(3);
        result.Total.Should().Be(1500 * 3);

        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockProductRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var command = new CreateOrderCommand(999, 3);

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(command.ProductId))
            .ReturnsAsync((Product?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithInsufficientStock_ShouldThrowInsufficientStockException()
    {
        // Arrange
        var product = Product.Create("Laptop", "High-end laptop", 1500, 5);
        var command = new CreateOrderCommand(product.Id, 10);

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(command.ProductId))
            .ReturnsAsync(product);

        _mockDomainService
            .Setup(x => x.CreateOrder(product, command.Quantity))
            .Throws(new InsufficientStockException("Insufficient stock for product."));

        // Act
        InsufficientStockException? caughtException = null;
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (InsufficientStockException ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException!.Message.Should().Contain("Insufficient stock");
    }
}

