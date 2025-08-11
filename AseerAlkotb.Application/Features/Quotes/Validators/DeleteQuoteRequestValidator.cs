using AseerAlkotb.Application.Features.Quotes.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class DeleteQuoteRequestValidator : AbstractValidator<DeleteQuoteRequest>
    {
        public DeleteQuoteRequestValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Quote Id can not be empty");
        }
    }
}
