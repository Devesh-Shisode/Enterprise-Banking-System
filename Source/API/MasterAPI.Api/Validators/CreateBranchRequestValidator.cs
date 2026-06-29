using FluentValidation;
using MasterAPI.Api.Models.Requests;

namespace MasterAPI.Api.Validators;

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.BranchCode)
            .NotEmpty().WithMessage("BranchCode is required.")
            .MaximumLength(20).WithMessage("BranchCode cannot exceed 20 characters.");

        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("BranchName is required.")
            .MaximumLength(100).WithMessage("BranchName cannot exceed 100 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.");
    }
}
