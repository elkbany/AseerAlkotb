using AseerAlkotb.Application.Features.Books.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class DeleteBookRequestValidator : AbstractValidator<DeleteBookRequest>
    {
        public DeleteBookRequestValidator()
        {
            RuleFor(request => request.Id).NotEmpty().GreaterThan(0).WithMessage("Book ID is required.");
        }
    }
}
