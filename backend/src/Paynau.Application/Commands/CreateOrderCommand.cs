
namespace Paynau.Application.Commands;

using MediatR;
using Paynau.Application.DTOs;

public record CreateOrderCommand(int ProductId, int Quantity) : IRequest<OrderDto>;

