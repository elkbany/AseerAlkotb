using AseerAlkotb.Domain.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public class ProcessPaymentResponse
    {
        public string RedirectUrl { get; set; }
    }
}
