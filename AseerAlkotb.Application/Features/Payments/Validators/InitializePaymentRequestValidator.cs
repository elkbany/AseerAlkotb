using AseerAlkotb.Application.Features.Payments.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Payments.Validators
{
    public class InitializePaymentRequestValidator : AbstractValidator<InitializePaymentRequest>
    {
        public InitializePaymentRequestValidator()
        {
            RuleFor(x => x.order.Id)
                .GreaterThan(0)
                .WithMessage("Order ID must be greater than 0");



        }
    }
}