using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class AddQuoteRequestValidator : AbstractValidator<AddQuoteRequest>
    {
        public AddQuoteRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("Quote", "EitherBookIdOrAuthorId");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .L("Comment", "Required")
                .MaximumLength(1000)
                .L("Comment", "CannotExceed", "1000", "Characters");
        }
    }
}
