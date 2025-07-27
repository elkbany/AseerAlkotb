using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class OrderItem: Entity<int> 
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1; // Default quantity is 1
        public decimal TotalPrice => UnitPrice * Quantity;
    
    
    }
}
