using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using AseerAlkotb.Application.ResponseHandler; // للـ ValidationExtensions
using AseerAlkotb.Domain.Resources;
using Microsoft.Extensions.Localization;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class DeleteSubCategoryRequestValidator : AbstractValidator<DeleteSubCategoryRequest>
    {
        public DeleteSubCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().L("SubCategory", "Id", "Required")
                .GreaterThan(0).L("SubCategory", "Id", "MustBeGreaterThan" , "0");

            RuleFor(x => x.ParentCategoryId)
                .NotEmpty().L("ParentCategory", "Id", "Required")
                .GreaterThan(0).L("ParentCategory", "Id", "MustBeGreaterThan" , "0");
        }
    }
}
