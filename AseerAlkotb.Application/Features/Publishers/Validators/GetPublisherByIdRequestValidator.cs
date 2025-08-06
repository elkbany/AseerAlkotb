using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetPublisherByIdRequestValidator : AbstractValidator<GetPublisherByIdRequest>
    {
        public GetPublisherByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Publisher ID must be greater than 0");
        }

    }
}
