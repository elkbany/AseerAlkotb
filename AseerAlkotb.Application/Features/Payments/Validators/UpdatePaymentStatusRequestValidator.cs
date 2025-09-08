using AseerAlkotb.Application.Features.Payments.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Payments.Validators
{
    public class UpdatePaymentStatusRequestValidator : AbstractValidator<UpdatePaymentStatusRequest>
    {
        public UpdatePaymentStatusRequestValidator()
        {
            RuleFor(x => x.PaymentId)
                .GreaterThan(0)
                .WithMessage("Payment ID must be greater than 0");

            RuleFor(x => x.NewStatus)
                .IsInEnum()
                .WithMessage("Invalid payment status");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters");
        }
    }
}