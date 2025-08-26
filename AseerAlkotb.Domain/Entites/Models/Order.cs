using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Order : Entity<int>
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; } 
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public EgyptGovernorates Governorate { get; set; }
        public EgyptCities City { get; set; }
        public OrderStatus Status { get; set; }
        public string TrackingNumber { get; set; } // What i Added Assuming a tracking number for the order

        #region Navigation Properties

        public int UserId { get; set; }
        public virtual User User { get; set; } // Assuming a User entity exists
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int? PayId { get; set; }
        public virtual Payment? Payment { get; set; } 
        #endregion

    }


}
