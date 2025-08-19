using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public class ProcessPaymentRequest
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
