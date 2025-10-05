
namespace Paynau.Tests.Application;

using FluentAssertions;
using Moq;
using Paynau.Application.Commands;
using Paynau.Application.Handlers;
using Paynau.Application.Interfaces;
using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;
using Xunit;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _handler = new CreateProductHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand("Laptop", "High-end laptop", 1500, 10);
        
        _mockRepository
            .Setup(x => x.ExistsByNameAsync(command.Name, null))
            .ReturnsAsync(false);
        
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Laptop");
        result.Price.Should().Be(1500);
        result.Stock.Should().Be(10);
        
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldThrowDuplicateProductException()
    {
        // Arrange
        var command = new CreateProductCommand("Laptop", "High-end laptop", 1500, 10);
        
        _mockRepository
            .Setup(x => x.ExistsByNameAsync(command.Name, null))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateProductException>();
    }
}

