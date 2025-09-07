﻿﻿﻿using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Payment : Entity<int>
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public long? PaymobOrderId { get; set; }
        public string? TransactionId { get; set; }
        public string ProviderPayload { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
