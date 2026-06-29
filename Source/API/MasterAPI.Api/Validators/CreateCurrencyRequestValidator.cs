using FluentValidation;
using MasterAPI.Api.Models.Requests;

namespace MasterAPI.Api.Validators;

public class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
{
    public CreateCurrencyRequestValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .Length(3)
            .WithMessage("CurrencyCode must be a 3-character ISO code (e.g. USD, EUR).");

        RuleFor(x => x.CurrencyName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Symbol)
            .NotEmpty()
            .MaximumLength(5);

        RuleFor(x => x.ExchangeRateToBase)
            .GreaterThan(0)
            .WithMessage("ExchangeRateToBase must be greater than 0.");
    }
}
