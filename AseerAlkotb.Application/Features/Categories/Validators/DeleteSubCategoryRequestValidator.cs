using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class DeleteSubCategoryRequestValidator: AbstractValidator<DeleteSubCategoryRequest>
    {
        public DeleteSubCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Category ID must be greater than 0");
            RuleFor(x => x.ParentCategoryId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("ParentCategoryId is required for subcategories.");
        }
    }
}
