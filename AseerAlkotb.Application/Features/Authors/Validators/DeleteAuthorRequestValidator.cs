using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Resources;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Authors.Validators
{
    public class DeleteAuthorRequestValidator : AbstractValidator<DeleteAuthorRequest>
    {
        public DeleteAuthorRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .L("Author", "Id", "Required")
                .GreaterThan(0)
                .L("Author", "Id", "MustBeGreaterThan", "0");
        }
    }
}
