
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>;

