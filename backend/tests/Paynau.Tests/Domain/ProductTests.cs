
namespace Paynau.Tests.Domain;

using FluentAssertions;
using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;
using Xunit;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange & Act
        var product = Product.Create("Laptop", "High-end laptop", 1500, 10);

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be("Laptop");
        product.Description.Should().Be("High-end laptop");
        product.Price.Should().Be(1500);
        product.Stock.Should().Be(10);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => Product.Create("", "Description", 100, 10);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Product name is required");
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => Product.Create("Product", "Description", -100, 10);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Product price cannot be negative");
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => Product.Create("Product", "Description", 100, -5);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Product stock cannot be negative");
    }

    [Fact]
    public void DecreaseStock_WithSufficientStock_ShouldDecreaseStock()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 10);

        // Act
        product.DecreaseStock(5);

        // Assert
        product.Stock.Should().Be(5);
    }

    [Fact]
    public void DecreaseStock_WithInsufficientStock_ShouldThrowInsufficientStockException()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 5);

        // Act
        var act = () => product.DecreaseStock(10);

        // Assert
        act.Should().Throw<InsufficientStockException>();
    }

    [Fact]
    public void DecreaseStock_WithZeroQuantity_ShouldThrowInvalidQuantityException()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 10);

        // Act
        var act = () => product.DecreaseStock(0);

        // Assert
        act.Should().Throw<InvalidQuantityException>();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 10);

        // Act
        product.Update("Updated Product", "Updated Description", 150, 20);

        // Assert
        product.Name.Should().Be("Updated Product");
        product.Description.Should().Be("Updated Description");
        product.Price.Should().Be(150);
        product.Stock.Should().Be(20);
    }

    [Fact]
    public void HasSufficientStock_WithEnoughStock_ShouldReturnTrue()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 10);

        // Act
        var result = product.HasSufficientStock(5);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasSufficientStock_WithInsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var product = Product.Create("Product", "Description", 100, 5);

        // Act
        var result = product.HasSufficientStock(10);

        // Assert
        result.Should().BeFalse();
    }
}

