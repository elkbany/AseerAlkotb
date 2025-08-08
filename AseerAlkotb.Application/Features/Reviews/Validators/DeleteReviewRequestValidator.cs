using AseerAlkotb.Application.Features.Reviews.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class DeleteReviewRequestValidator : AbstractValidator<DeleteReviewRequest>
    {
        public DeleteReviewRequestValidator() 
        {

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Review ID cannot be empty.");
         
        }
    }
}
