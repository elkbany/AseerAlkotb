using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.ResponseHandler;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Quotes.Validators
{
    public class UpdateQuoteRequestValidator : AbstractValidator<UpdateQuoteRequest>
    {
        public UpdateQuoteRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .L("Quote" , "Id", "NotEmpty");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .L("Quote" , "Comment", "Required")
                .MaximumLength(1000)
                .L("Quote" , "Comment", "MaxLength", "1000");
        }
    }
}
