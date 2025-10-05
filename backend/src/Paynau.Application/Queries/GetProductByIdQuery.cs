
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

