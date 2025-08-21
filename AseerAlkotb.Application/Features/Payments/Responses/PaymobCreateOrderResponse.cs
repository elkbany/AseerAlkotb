using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public class PaymobCreateOrderResponse
    {
        public int id { get; set; }       // رقم الأوردر عند Paymob
        public bool pending { get; set; }
    }

}
