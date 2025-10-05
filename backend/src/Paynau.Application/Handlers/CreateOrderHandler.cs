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
        catch (ConcurrencyException)
        {
            throw new ConcurrencyException("The product stock was modified by another transaction. Please try again.");
        }
        catch (Exception ex)
        {
            throw new DomainException($"Unexpected error while creating order: {ex.Message}", ex);
        }
    }
}

