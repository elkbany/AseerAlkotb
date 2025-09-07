using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Wishlist.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Wishlist.Validators
{
    public class ClearWishlistValidation : AbstractValidator<ClearWishlistRequest>
    {
        public ClearWishlistValidation()
        {
           
        }
    }
}
