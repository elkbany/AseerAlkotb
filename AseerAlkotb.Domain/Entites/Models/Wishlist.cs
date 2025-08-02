using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Wishlist : Entity<int>
    {
        #region Navigation Properties
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<WishlistItem> WishlistItems { get; set; }

        #endregion
    }
}
