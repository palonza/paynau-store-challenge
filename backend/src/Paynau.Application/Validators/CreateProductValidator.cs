
namespace Paynau.Application.Validators;

using FluentValidation;
using Paynau.Application.Commands;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Product price cannot be negative");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative");
    }
}

