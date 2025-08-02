using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Review: Entity<int>
    {
      
        public string Comment { get; set; }
        public int Rating { get; set; } // Assuming rating is an integer value

        #region Navigation Properties
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        #endregion
    }


}
