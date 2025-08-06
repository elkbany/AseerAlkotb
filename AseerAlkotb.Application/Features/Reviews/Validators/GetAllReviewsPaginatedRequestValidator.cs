

using AseerAlkotb.Application.Features.Reviews.Requests;
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
                 .WithMessage("Either BookId or AuthorId must be provided, but not both.");
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}
