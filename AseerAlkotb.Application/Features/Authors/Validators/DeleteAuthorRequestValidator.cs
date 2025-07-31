using AseerAlkotb.Application.Features.Authors.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class DeleteAuthorRequestValidator : AbstractValidator<DeleteAuthorRequest>
    {
        public DeleteAuthorRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Author ID must be greater than 0");
        }
    }
}
