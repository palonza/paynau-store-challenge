
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

