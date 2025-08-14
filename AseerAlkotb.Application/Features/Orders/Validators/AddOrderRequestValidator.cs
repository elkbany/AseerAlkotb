using FluentValidation;
using AseerAlkotb.Application.Features.Orders.Requests;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class AddOrderRequestValidator : AbstractValidator<AddOrderRequest>
    {
        public AddOrderRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be a positive number.");

            RuleFor(x => x.Governorate)
                .IsInEnum().WithMessage("Invalid governorate.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");

           
        }
    }
}
