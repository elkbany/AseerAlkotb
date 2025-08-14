using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public record InitiatePaymentResponse(
        string RedirectUrl,
        bool Success,
        string Message,
        string TransactionId
        );
}
