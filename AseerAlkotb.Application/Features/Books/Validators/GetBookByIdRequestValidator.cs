using AseerAlkotb.Application.Features.Books.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Validators
{
    public class GetBookByIdRequestValidator : AbstractValidator<GetBookByIdRequest>
    {
        public GetBookByIdRequestValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Book ID must be greater than 0.");
        }
    }
}
        