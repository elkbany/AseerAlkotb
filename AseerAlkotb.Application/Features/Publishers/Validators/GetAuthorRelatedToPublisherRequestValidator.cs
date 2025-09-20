using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Publishers.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Publishers.Validators
{
    public class GetAuthorRelatedToPublisherRequestValidator : AbstractValidator<GetAuthorRelatedToPublisherRequest>
    {
        public GetAuthorRelatedToPublisherRequestValidator()
        {
            RuleFor(x => x.publisherId)
              .NotEmpty()
              .GreaterThan(0)
             .WithMessage("Publisher ID must be greater than 0");
        }
    }
}
