using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Approved = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
