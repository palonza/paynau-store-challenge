// ========================================
// Paynau.Tests.csproj
// ========================================
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
    <PackageReference Include="xunit" Version="2.9.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.8" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\src\Paynau.Domain\Paynau.Domain.csproj" />
    <ProjectReference Include="..\src\Paynau.Application\Paynau.Application.csproj" />
    <ProjectReference Include="..\src\Paynau.Infrastructure\Paynau.Infrastructure.csproj" />
    <ProjectReference Include="..\src\Paynau.Api\Paynau.Api.csproj" />
  </ItemGroup>
</Project>

// ========================================
// Domain/ProductTests.cs
// ========================================
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

// ========================================
// Domain/OrderDomainServiceTests.cs
// ========================================
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

// ========================================
// Application/CreateProductHandlerTests.cs
// ========================================
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

// ========================================
// Application/CreateOrderHandlerTests.cs
// ========================================
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
            .Throws<InsufficientStockException>();

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InsufficientStockException>();
    }
}

// ========================================
// Application/ValidatorTests.cs
// ========================================
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
