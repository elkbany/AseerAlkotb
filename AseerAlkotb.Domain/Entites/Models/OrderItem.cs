using AseerAlkotb.Domain.Entites.Base;
namespace AseerAlkotb.Domain.Entites.Models
{
    public class OrderItem
    {

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1; // Default quantity is 1
        public decimal TotalPrice => UnitPrice * Quantity;
        #region Navigation Properties
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Book Book { get; set; }
        public int BookId { get; set; }
        #endregion


    } 
}
