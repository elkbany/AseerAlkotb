using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Wishlist.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Wishlist.Validators
{
    public class DeleteWishlistItemValidation : AbstractValidator<DeleteWishlistItemRequest>
    {
        public DeleteWishlistItemValidation()
        {
           

            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Book ID must be greater than 0");
        }
    }
}
