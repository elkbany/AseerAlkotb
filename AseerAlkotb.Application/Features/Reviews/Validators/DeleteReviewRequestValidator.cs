using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class DeleteReviewRequestValidator : AbstractValidator<DeleteReviewRequest>
    {
        public DeleteReviewRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .L("ReviewId", "Required");
        }
    }
}
