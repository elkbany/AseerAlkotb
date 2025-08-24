using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Domain.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;
using AseerAlkotb.Application.ResponseHandler; // للـ ValidationExtensions

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class AddCategoryRequestValidator : AbstractValidator<AddCategoryRequest>
    {
        public AddCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().L("Category", "Name", "Required")
                .Length(2, 100).L("Category", "Name", "MustBeBetween", "2", "100")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").L("Category", "Name", "LettersOnly");

            RuleFor(x => x.Description)
                .MaximumLength(1000).L("Description", "MaxLength", "1000");

            RuleFor(x => x.IsActive)
                .NotNull().L("IsActive", "Required");
        }
    }
}
