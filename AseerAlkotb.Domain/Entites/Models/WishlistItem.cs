using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class WishlistItem
    {
        #region Navigation Properties
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public  virtual Wishlist Wishlist { get; set; }
        public int WishlistId { get; set; } 
        #endregion

    }
}
