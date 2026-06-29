using FluentValidation;
using AccountAPI.Api.Models.Requests;

namespace AccountAPI.Api.Validators;

public class UpdateAccountStatusRequestValidator : AbstractValidator<UpdateAccountStatusRequest>
{
    private static readonly string[] AllowedStatuses = { "Active", "Suspended", "Dormant", "Closed" };

    public UpdateAccountStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => System.Array.Exists(AllowedStatuses, s => s.Equals(status, System.StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Status must be Active, Suspended, Dormant, or Closed.");
    }
}
