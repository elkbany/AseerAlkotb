using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Review: Entity<int>
    {
      
        public string? Comment { get; set; }
        public int Rating { get; set; } // Assuming rating is an integer value


        #region Navigation Properties
        public ReviewFor ReviewType { get; set; }
        public int? BookId { get; set; }
        public virtual Book Book { get; set; }
        public int? ReviewAuthorId { get; set; }
        public virtual Author Author { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<LikeDisLike> LikeDisLikes { get; set; } = [];
        #endregion
    }


}
