using FluentValidation;
using AseerAlkotb.Application.Features.OrderItems.DTOs;

namespace AseerAlkotb.Application.Features.Orders.Validators
{
    public class OrderItemDTOValidator : AbstractValidator<OrderItemDTO>
    {
        public OrderItemDTOValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("BookId must be a positive number.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
