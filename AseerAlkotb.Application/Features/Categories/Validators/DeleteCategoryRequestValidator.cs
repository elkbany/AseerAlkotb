using AseerAlkotb.Application.Features.Categories.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Validators
{
    public class DeleteCategoryRequestValidator : AbstractValidator<DeleteCategoryRequest>
    {
        public DeleteCategoryRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Category ID must be greater than 0");
        }
    }
}