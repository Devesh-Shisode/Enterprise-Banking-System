using FluentValidation;
using AccountAPI.Api.Models.Requests;

namespace AccountAPI.Api.Validators;

public class UpdateAccountLimitsRequestValidator : AbstractValidator<UpdateAccountLimitsRequest>
{
    public UpdateAccountLimitsRequestValidator()
    {
        RuleFor(x => x.DailyTransferLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("DailyTransferLimit cannot be negative.");

        RuleFor(x => x.DailyWithdrawalLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("DailyWithdrawalLimit cannot be negative.");

        RuleFor(x => x.TransactionLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TransactionLimit cannot be negative.");
    }
}
