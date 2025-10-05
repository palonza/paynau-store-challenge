
namespace Paynau.Application.Commands;

using MediatR;

public record DeleteProductCommand(int Id) : IRequest<Unit>;

