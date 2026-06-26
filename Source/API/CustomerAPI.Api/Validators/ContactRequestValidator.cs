using FluentValidation;
using CustomerAPI.Api.Models.Requests;

namespace CustomerAPI.Api.Validators;

public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
    public ContactRequestValidator()
    {
        RuleFor(x => x.ContactType)
            .NotEmpty().WithMessage("Contact type is required.")
            .Must(x => x == "Email" || x == "Mobile" || x == "HomePhone" || x == "WorkPhone")
            .WithMessage("Contact type must be 'Email', 'Mobile', 'HomePhone', or 'WorkPhone'.");

        RuleFor(x => x.ContactValue)
            .NotEmpty().WithMessage("Contact value is required.")
            .MaximumLength(100).WithMessage("Contact value cannot exceed 100 characters.");
    }
}
