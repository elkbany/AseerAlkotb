using AseerAlkotb.Application.Features.Orders.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Order ID must be greater than 0");

            RuleFor(x => x.NewStatus)
                .IsInEnum()
                .WithMessage("Invalid order status");
        }
    }
}