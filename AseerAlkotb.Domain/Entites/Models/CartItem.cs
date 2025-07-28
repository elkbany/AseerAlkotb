using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class CartItem : Entity<int>
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } // Navigation property to Book entity
        public int Quantity { get; set; } = 1; // Default quantity is 1
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        //public User user { get; set; } // Navigation property to User entity
    }
    
    
}
