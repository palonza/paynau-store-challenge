
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

