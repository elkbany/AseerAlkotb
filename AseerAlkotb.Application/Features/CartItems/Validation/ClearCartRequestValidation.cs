using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.CartItems.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.CartItems.Validation
{
    public class ClearCartRequestValidation: AbstractValidator<ClearCartRequest>
    {
        public ClearCartRequestValidation()
        {
         
        }
    }
}
