// ========================================
// Paynau.Application.csproj
// ========================================
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.0" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\Paynau.Domain\Paynau.Domain.csproj" />
  </ItemGroup>
</Project>

// ========================================
// DTOs/ProductDto.cs
// ========================================
namespace Paynau.Application.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Stock
);

// ========================================
// DTOs/CreateProductDto.cs
// ========================================
namespace Paynau.Application.DTOs;

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock
);

// ========================================
// DTOs/UpdateProductDto.cs
// ========================================
namespace Paynau.Application.DTOs;

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock
);

// ========================================
// DTOs/OrderDto.cs
// ========================================
namespace Paynau.Application.DTOs;

public record OrderDto(
    int Id,
    int ProductId,
    int Quantity,
    decimal Total,
    DateTime CreatedAt
);

// ========================================
// DTOs/CreateOrderDto.cs
// ========================================
namespace Paynau.Application.DTOs;

public record CreateOrderDto(
    int ProductId,
    int Quantity
);

// ========================================
// Interfaces/IProductRepository.cs
// ========================================
namespace Paynau.Application.Interfaces;

using Paynau.Domain.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task SaveChangesAsync();
}

// ========================================
// Interfaces/IOrderRepository.cs
// ========================================
namespace Paynau.Application.Interfaces;

using Paynau.Domain.Entities;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> AddAsync(Order order);
    Task<bool> HasOrdersForProductAsync(int productId);
    Task SaveChangesAsync();
}

// ========================================
// Commands/CreateProductCommand.cs
// ========================================
namespace Paynau.Application.Commands;

using MediatR;
using Paynau.Application.DTOs;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock) 
    : IRequest<ProductDto>;

// ========================================
// Commands/UpdateProductCommand.cs
// ========================================
namespace Paynau.Application.Commands;

using MediatR;
using Paynau.Application.DTOs;

public record UpdateProductCommand(int Id, string Name, string Description, decimal Price, int Stock) 
    : IRequest<ProductDto>;

// ========================================
// Commands/DeleteProductCommand.cs
// ========================================
namespace Paynau.Application.Commands;

using MediatR;

public record DeleteProductCommand(int Id) : IRequest<Unit>;

// ========================================
// Commands/CreateOrderCommand.cs
// ========================================
namespace Paynau.Application.Commands;

using MediatR;
using Paynau.Application.DTOs;

public record CreateOrderCommand(int ProductId, int Quantity) : IRequest<OrderDto>;

// ========================================
// Queries/GetAllProductsQuery.cs
// ========================================
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

// ========================================
// Queries/GetProductByIdQuery.cs
// ========================================
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

// ========================================
// Queries/GetAllOrdersQuery.cs
// ========================================
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>;

// ========================================
// Handlers/CreateProductHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Domain.Entities;
using Paynau.Domain.Exceptions;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (await _repository.ExistsByNameAsync(request.Name))
            throw new DuplicateProductException(request.Name);

        var product = Product.Create(request.Name, request.Description, request.Price, request.Stock);
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}

// ========================================
// Handlers/UpdateProductHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Domain.Exceptions;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;

    public UpdateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
            throw new ProductNotFoundException(request.Id);

        if (await _repository.ExistsByNameAsync(request.Name, request.Id))
            throw new DuplicateProductException(request.Name);

        product.Update(request.Name, request.Description, request.Price, request.Stock);
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}

// ========================================
// Handlers/DeleteProductHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.Commands;
using Paynau.Application.Interfaces;
using Paynau.Domain.Exceptions;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public DeleteProductHandler(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
            throw new ProductNotFoundException(request.Id);

        if (await _orderRepository.HasOrdersForProductAsync(request.Id))
            throw new ProductInUseException(request.Id);

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();

        return Unit.Value;
    }
}

// ========================================
// Handlers/CreateOrderHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Domain.Exceptions;
using Paynau.Domain.Interfaces;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDomainService _orderDomainService;

    public CreateOrderHandler(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IOrderDomainService orderDomainService)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _orderDomainService = orderDomainService;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new ProductNotFoundException(request.ProductId);

        try
        {
            var order = _orderDomainService.CreateOrder(product, request.Quantity);
            await _orderRepository.AddAsync(order);
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return new OrderDto(order.Id, order.ProductId, order.Quantity, order.Total, order.CreatedAt);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("The product stock was modified by another transaction. Please try again.");
        }
    }
}

// ========================================
// Handlers/GetAllProductsHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Application.Queries;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock));
    }
}

// ========================================
// Handlers/GetProductByIdHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Application.Queries;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        return product == null 
            ? null 
            : new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }
}

// ========================================
// Handlers/GetAllOrdersHandler.cs
// ========================================
namespace Paynau.Application.Handlers;

using MediatR;
using Paynau.Application.DTOs;
using Paynau.Application.Interfaces;
using Paynau.Application.Queries;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetAllOrdersHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken ct)
    {
        var orders = await _repository.GetAllAsync();
        return orders.Select(o => new OrderDto(o.Id, o.ProductId, o.Quantity, o.Total, o.CreatedAt));
    }
}

// ========================================
// Validators/CreateProductValidator.cs
// ========================================
namespace Paynau.Application.Validators;

using FluentValidation;
using Paynau.Application.Commands;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Product price cannot be negative");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative");
    }
}

// ========================================
// Validators/UpdateProductValidator.cs
// ========================================
namespace Paynau.Application.Validators;

using FluentValidation;
using Paynau.Application.Commands;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Product price cannot be negative");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative");
    }
}

// ========================================
// Validators/CreateOrderValidator.cs
// ========================================
namespace Paynau.Application.Validators;

using FluentValidation;
using Paynau.Application.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Order quantity must be greater than zero");
    }
}
