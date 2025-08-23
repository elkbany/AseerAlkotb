using AseerAlkotb.Application.Features.Quotes.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class GetAllQuotePaginatedRequestValidator : AbstractValidator<GetAllQuotesPaginatedRequest>
    {
        public GetAllQuotePaginatedRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .WithMessage("Either BookId or AuthorId must be provided, but not both.");

            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");

        }
    }
}
