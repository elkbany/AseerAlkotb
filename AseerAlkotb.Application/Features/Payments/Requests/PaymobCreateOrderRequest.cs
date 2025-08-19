using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public class PaymobCreateOrderRequest
    {
        public string auth_token { get; set; }
        public int amount_cents { get; set; }   // Paymob بيستخدم الفلوس بالـ "قرش" (100 = 1 جنيه)
        public string currency { get; set; } = "EGP";
        public int merchant_order_id { get; set; } // رقم الأوردر عندك في السيستم
    }
}
