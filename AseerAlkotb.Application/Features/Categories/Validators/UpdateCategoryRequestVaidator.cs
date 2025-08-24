using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ Extension L
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("CategoryId", "MustBeGreaterThanZero");

            RuleFor(x => x.Name)
                .NotEmpty()
                .L("CategoryName", "Required")
                .Length(2, 100)
                .L("CategoryName", "MustBeBetween", "2" , "100")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$")
                .L("CategoryName", "LettersOnly");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .L("Description", "MaxLength", "1000");

            RuleFor(x => x.IsActive)
                .NotNull()
                .L("IsActive", "Required");
        }
    }
}
