
namespace Paynau.Tests.Application;

using FluentAssertions;
using Paynau.Application.Commands;
using Paynau.Application.Validators;
using Xunit;

public class ValidatorTests
{
    [Fact]
    public void CreateProductValidator_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateProductCommand("Laptop", "High-end laptop", 1500, 10);
        var validator = new CreateProductValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateProductValidator_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand("", "Description", 1500, 10);
        var validator = new CreateProductValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateProductValidator_WithNegativePrice_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand("Product", "Description", -100, 10);
        var validator = new CreateProductValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void CreateOrderValidator_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateOrderCommand(1, 5);
        var validator = new CreateOrderValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateOrderValidator_WithZeroQuantity_ShouldHaveError()
    {
        // Arrange
        var command = new CreateOrderCommand(1, 0);
        var validator = new CreateOrderValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }
}
