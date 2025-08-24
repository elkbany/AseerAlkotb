using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class DeleteBookRequestValidator : AbstractValidator<DeleteBookRequest>
    {
        public DeleteBookRequestValidator()
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .L("Book", "Id", "Required")
                .GreaterThan(0)
                .L("Book", "Id", "MustBeGreaterThan", "0");
        }
    }
}
