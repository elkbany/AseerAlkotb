using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required")
                .Length(2, 100)
                .WithMessage("Category name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .WithMessage("Category name can only contain letters and spaces");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("IsActive status must be specified");

        }
    }
}
