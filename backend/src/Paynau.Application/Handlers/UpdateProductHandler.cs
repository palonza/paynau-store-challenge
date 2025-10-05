
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

