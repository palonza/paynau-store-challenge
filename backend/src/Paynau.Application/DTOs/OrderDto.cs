
namespace Paynau.Application.DTOs;

public record OrderDto(
    int Id,
    int ProductId,
    int Quantity,
    decimal Total,
    DateTime CreatedAt
);

