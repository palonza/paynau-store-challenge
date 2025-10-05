
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

