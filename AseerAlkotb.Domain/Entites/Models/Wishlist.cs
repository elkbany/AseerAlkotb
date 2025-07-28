using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Wishlist : Entity<int>
    {
        #region Navigation Properties
        //public int UserId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        #endregion
    }
}
