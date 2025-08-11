using AseerAlkotb.Application.Features.Quotes.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class GetByIdQuoteRequestValidator : AbstractValidator<GetQuoteByIdRequest>
    {
        public GetByIdQuoteRequestValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Quote Id can not be empty.");

            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .WithMessage("Either BookId or AuthorId must be provided, but not both.");

        }
    }
}
