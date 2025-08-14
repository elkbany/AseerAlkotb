using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Payment : Entity<int>
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public PaymentMethod Method { get; set; }      // CashOnDelivery, MobileWallet, Paypal
        public PaymentStatus Status { get; set; }      // Pending, Completed, Failed, ...
        public DateTime PaymentDate { get; set;} 
        public decimal Amount { get; set; }
        public string Provider { get; set; }           // "PayPal" | "Paymob" | "COD"
        public string TransactionId { get; set; }      // ID من مزود الدفع
        public string ProviderPayload { get; set; }    // JSON خام (اختياري للتتبع)
    }
}
