using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .L("ReviewId", "CannotBeEmpty");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .L("Rating", "MustBeBetween1And5");

            RuleFor(x => x.Comment)
                .NotEmpty().L("Comment", "CannotBeEmpty")
                .MaximumLength(2000).L("Comment", "MaxLength2000");
        }
    }
}
