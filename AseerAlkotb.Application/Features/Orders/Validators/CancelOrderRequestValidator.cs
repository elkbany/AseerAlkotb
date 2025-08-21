using FluentValidation;
using AseerAlkotb.Application.Features.Orders.Requests;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class CancelOrderRequestValidator : AbstractValidator<CancelOrderRequest>
    {
        public CancelOrderRequestValidator()
        {
            RuleFor(x => x.TrackingNumber)
                 .NotEmpty()
                 .NotNull()
                 .WithMessage("Tracking Number Cannot be null or empty");
        }
    }
}
