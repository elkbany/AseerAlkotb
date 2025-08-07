using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class AddSubCategoryRequestValidator : AbstractValidator<AddSubCategoryRequest>
    {
        public AddSubCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100)
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$");

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0)
                .WithMessage("ParentCategoryId is required and must be greater than 0");
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("IsActive status must be specified");
        }
    }
}
