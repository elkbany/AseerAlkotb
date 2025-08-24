using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ ValidationExtensions
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class AddSubCategoryRequestValidator : AbstractValidator<AddSubCategoryRequest>
    {
        public AddSubCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().L("SubCategory", "Name", "Required")
                .Length(2, 100).L("SubCategory", "Name", "MustBeBetween", "2", "100")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").L("SubCategory", "Name", "LettersOnly");

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0).L("ParentCategory", "Id", "MustBeGreaterThan" , "0");

            RuleFor(x => x.Description)
                .MaximumLength(1000).L("Description", "MaxLength", "1000");

            RuleFor(x => x.IsActive)
                .NotNull().L("IsActive", "Required");
        }
    }
}
