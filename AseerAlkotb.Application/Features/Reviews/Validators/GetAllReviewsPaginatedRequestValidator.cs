using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class GetAllReviewsPaginatedRequestValidator : AbstractValidator<GetAllReviewsPaginatedRequest>
    {
        public GetAllReviewsPaginatedRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("BookOrAuthor", "EitherProvided"); // Localized message

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .L("PageNumber", "MustBeGreaterThan" , "0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .L("PageSize", "MustBeBetween" , "1" , "100");
        }
    }
}
