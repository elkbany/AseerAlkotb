using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ ValidationExtensions
using AseerAlkotb.Localization.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class DeleteCategoryRequestValidator : AbstractValidator<DeleteCategoryRequest>
    {
        public DeleteCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().L("Category", "Id", "Required")
                .GreaterThan(0).L("Category", "Id", "MustBeGreaterThan" , "0");
        }
    }
}
