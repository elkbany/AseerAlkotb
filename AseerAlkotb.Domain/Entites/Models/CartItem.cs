using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class CartItem 
    {
    
        public int Quantity { get; set; } = 1; // Default quantity is 1
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        #region Navigation Properties
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public virtual Cart Cart { get; set; }
        public int CartId { get; set; }

        #endregion
    }
    
    
}
