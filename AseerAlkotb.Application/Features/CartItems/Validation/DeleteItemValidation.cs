using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.CartItems.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.CartItems.Validation
{
    public class DeleteItemValidation : AbstractValidator<DeleteItemRequest>
    {
        public DeleteItemValidation() 
        {
          

            RuleFor(x => x.bookId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Book ID must be greater than 0");
        }
    }
}
