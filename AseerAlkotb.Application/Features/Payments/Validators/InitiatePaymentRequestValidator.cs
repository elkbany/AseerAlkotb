using AseerAlkotb.Application.Features.Payments.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Validators
{
    public class InitiatePaymentRequestValidator : AbstractValidator<InitiatePaymentRequest>
    {
        public InitiatePaymentRequestValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.Method).IsInEnum();
        }
    }
}
