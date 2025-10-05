
namespace Paynau.Application.DTOs;

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock
);

