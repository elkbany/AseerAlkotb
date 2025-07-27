using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Order : Entity<int>
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public string TrackingNumber { get; set; } // What i Added Assuming a tracking number for the order
        // Navigation properties
        public int UserId { get; set; }
        //public User User { get; set; } // Assuming a User entity exists
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    
    
}
