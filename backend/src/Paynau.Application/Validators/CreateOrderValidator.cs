
namespace Paynau.Application.Validators;

using FluentValidation;
using Paynau.Application.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Order quantity must be greater than zero");
    }
}
