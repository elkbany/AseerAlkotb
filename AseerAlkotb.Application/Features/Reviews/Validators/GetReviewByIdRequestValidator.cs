using AseerAlkotb.Application.Features.Reviews.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class GetReviewByIdRequestValidator : AbstractValidator<GetReviewByIdRequest>
    {
        public GetReviewByIdRequestValidator() 
        {

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Review ID cannot be empty.");
            RuleFor(x => x)
                 .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                 (!x.BookId.HasValue && x.AuthorId.HasValue))
                 .WithMessage("Either BookId or AuthorId must be provided, but not both.");
        }
    }
}
