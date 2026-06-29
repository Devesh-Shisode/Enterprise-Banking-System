using FluentValidation;
using AccountAPI.Api.Models.Requests;

namespace AccountAPI.Api.Validators;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    private static readonly string[] AllowedTypes = { "Savings", "Checking", "Loan" };

    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(type => System.Array.Exists(AllowedTypes, t => t.Equals(type, System.StringComparison.OrdinalIgnoreCase)))
            .WithMessage("AccountType must be Savings, Checking, or Loan.");

        RuleFor(x => x.InitialDeposit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("InitialDeposit cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a 3-character ISO code (e.g. USD, EUR).");
    }
}
