using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites
{
    public class Wishlist : Entity<int>
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } 
        public ICollection<OrderItem> WishlistItems { get; set; } = new List<OrderItem>();
    }
}
