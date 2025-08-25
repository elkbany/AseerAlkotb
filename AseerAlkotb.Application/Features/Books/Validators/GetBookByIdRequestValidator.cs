using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Localization.Resources;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
    {
        public GetBookByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .L("Book", "Id", "MustBeGreaterThanZero");
        }
    }
}
