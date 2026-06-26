using System;
using FluentValidation;
using CustomerAPI.Api.Models.Requests;

namespace CustomerAPI.Api.Validators;

public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
{
    public CustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(BeAtLeast18).WithMessage("Customer must be at least 18 years old.");

        RuleFor(x => x.PAN)
            .NotEmpty().WithMessage("PAN is required.")
            .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").WithMessage("A valid 10-character PAN card format is required.");

        RuleFor(x => x.AadhaarNumber)
            .NotEmpty().WithMessage("Aadhaar number is required.")
            .Matches(@"^[0-9]{12}$").WithMessage("A valid 12-digit Aadhaar number is required.");
    }

    private bool BeAtLeast18(DateTime dateOfBirth)
    {
        return dateOfBirth <= DateTime.Today.AddYears(-18);
    }
}
