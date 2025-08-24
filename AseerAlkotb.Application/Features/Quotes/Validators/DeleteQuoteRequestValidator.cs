using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class DeleteQuoteRequestValidator : AbstractValidator<DeleteQuoteRequest>
    {
        public DeleteQuoteRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .L("Quote", "Required");
        }
    }
}
