using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Reviews.Validators
{
    public class GetReviewByIdRequestValidator : AbstractValidator<GetReviewByIdRequest>
    {
        public GetReviewByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .L("Review" , "Id", "CannotBe" , "Empty"); 

            RuleFor(x => x)
                .Must(x => (x.BookId.HasValue && !x.AuthorId.HasValue) ||
                           (!x.BookId.HasValue && x.AuthorId.HasValue))
                .L("Book" , "Or" , "Author"); 
        }
    }
}
