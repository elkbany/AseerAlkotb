using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class GetAllQuotePaginatedRequestValidator : AbstractValidator<GetAllQuotesPaginatedRequest>
    {
        public GetAllQuotePaginatedRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("EitherBookIdOrAuthorId");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThan", "0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .L("PageSize", "MustBeGreaterThan", "0");
        }
    }
}
