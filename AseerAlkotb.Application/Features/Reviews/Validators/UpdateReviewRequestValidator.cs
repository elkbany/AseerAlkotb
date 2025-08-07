using AseerAlkotb.Application.Features.Reviews.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Review ID cannot be empty.");
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("Comment cannot be empty.")
                .MaximumLength(2000)
                .WithMessage("Comment cannot exceed 2000 characters.");
        }
    }
}
