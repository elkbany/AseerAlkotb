using AseerAlkotb.Application.Features.Quotes.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class UpdateQuoteRequestValidator : AbstractValidator<UpdateQuoteRequest>
    {
        public UpdateQuoteRequestValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Quote Id can not be empty.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");
        }
    }
}
