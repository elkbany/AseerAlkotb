using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class LikeDisLike : Entity<int>
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int ReviewId { get; set; }
        public virtual Review Review { get; set; }

        //public int? QuoteId { get; set; }
        //public Quote Quote { get; set; }
        public bool IsLike { get; set; }
    }
}
