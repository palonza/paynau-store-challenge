
namespace Paynau.Application.Commands;

using MediatR;
using Paynau.Application.DTOs;

public record UpdateProductCommand(int Id, string Name, string Description, decimal Price, int Stock) 
    : IRequest<ProductDto>;

