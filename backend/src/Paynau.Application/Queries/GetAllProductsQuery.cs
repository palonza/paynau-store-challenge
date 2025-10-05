
namespace Paynau.Application.Queries;

using MediatR;
using Paynau.Application.DTOs;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

