using FluentValidation;
using CustomerAPI.Api.Models.Requests;

namespace CustomerAPI.Api.Validators;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.AddressType)
            .NotEmpty().WithMessage("Address type is required.")
            .Must(x => x == "Permanent" || x == "Correspondence")
            .WithMessage("Address type must be 'Permanent' or 'Correspondence'.");

        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required.")
            .MaximumLength(100).WithMessage("Address line 1 cannot exceed 100 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(50).WithMessage("State cannot exceed 50 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .MaximumLength(15).WithMessage("Postal code cannot exceed 15 characters.");
    }
}
