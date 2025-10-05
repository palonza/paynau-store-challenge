
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

