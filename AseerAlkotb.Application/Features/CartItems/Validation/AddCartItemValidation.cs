using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.CartItems.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.CartItems.Validation
{
     public class AddCartItemValidation: AbstractValidator<AddItemToCartRequest>
    {
        public AddCartItemValidation() {

            RuleFor(x => x.UserId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("user ID must be greater than 0");

            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Book ID must be greater than 0");
        }
    }
}
