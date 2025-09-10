using FluentValidation;
using AseerAlkotb.Application.Features.Orders.Requests;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class AddOrderRequestValidator : AbstractValidator<AddOrderRequest>
    {
        public AddOrderRequestValidator()
        {
            //RuleFor(x => x.UserId)
            //    .GreaterThan(0).WithMessage("UserId must be a positive number.");

            RuleFor(x => x.GovernorateId)
                .NotEmpty().WithMessage("Invalid governorate.");
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("Invalid city.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");

           
        }
    }
}
