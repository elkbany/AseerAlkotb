using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    class GetPublisherFollowerCountRequestValidation :AbstractValidator<GetPublisherFollowerCountRequest>
    {
        public GetPublisherFollowerCountRequestValidation()
        {
            RuleFor(x => x.PublisherId)
             .NotEmpty()
             .GreaterThan(0)
            .WithMessage("Publisher ID must be greater than 0");
        }
    }
}
