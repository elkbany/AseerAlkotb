using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record InitiatePaymentRequest(
        int OrderId, 
        decimal Amount,
        PaymentMethod Method
    );
}
