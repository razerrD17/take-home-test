using FluentValidation;
using Fundo.Services.DTOs;

namespace Fundo.Services.Validators;

public class CreateLoanDtoValidator : AbstractValidator<CreateLoanDto>
{
    public CreateLoanDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(10_000_000m).WithMessage("Amount must not exceed 10,000,000");

        RuleFor(x => x.ApplicantName)
            .NotEmpty().WithMessage("Applicant name is required")
            .MinimumLength(2).WithMessage("Applicant name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Applicant name must not exceed 200 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Applicant name can only contain letters, spaces, hyphens, apostrophes, and periods");
    }
}
