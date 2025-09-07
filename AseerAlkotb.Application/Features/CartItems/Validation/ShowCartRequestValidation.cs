using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.CartItem.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.CartItems.Validation
{
    class ShowCartRequestValidation: AbstractValidator<ShowCartRequest>
    {
        public ShowCartRequestValidation()
        {
         
        }
    }
}
