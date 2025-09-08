using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Rag.Requests;
using FluentValidation;

namespace AseerAlkotb.Application.Features.Rag.Validators
{
    public class RagAskRequestValidator : AbstractValidator<RagAskRequest>
    {
        public RagAskRequestValidator()
        {
            RuleFor(x => x.Question).NotEmpty().MinimumLength(2);
            RuleFor(x => x.Limit).InclusiveBetween(1, 20);
        }
    }
}
