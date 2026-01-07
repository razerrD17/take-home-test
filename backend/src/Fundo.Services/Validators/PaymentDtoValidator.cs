using FluentValidation;
using Fundo.Services.DTOs;

namespace Fundo.Services.Validators;

public class PaymentDtoValidator : AbstractValidator<PaymentDto>
{
    public PaymentDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Payment amount is required")
            .GreaterThan(0).WithMessage("Payment amount must be greater than 0")
            .LessThanOrEqualTo(10_000_000m).WithMessage("Payment amount must not exceed 10,000,000");
    }
}
