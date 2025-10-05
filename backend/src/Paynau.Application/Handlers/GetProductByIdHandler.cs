
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

