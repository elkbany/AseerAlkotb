using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class AddReviewRequestValidator : AbstractValidator<AddReviewRequest>
    {
        public AddReviewRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("EitherBookIdOrAuthorId", "Required");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .L("Rating", "Range", "1", "5");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .L("Comment", "Required")
                .MaximumLength(2000)
                .L("Comment", "MaxLength", "2000");
        }
    }
}
