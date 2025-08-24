using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class GetByIdQuoteRequestValidator : AbstractValidator<GetQuoteByIdRequest>
    {
        public GetByIdQuoteRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .L("Quote" , "Id", "NotEmpty");

            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("EitherBookIdOrAuthorId");
        }
    }
}
