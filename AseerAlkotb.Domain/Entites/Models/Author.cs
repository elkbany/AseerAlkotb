using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Author : Entity<int>
    {
        public string Name { get; set; }
        public string? Name_en { get; set; } 
        public string Bio { get; set; }
        public string? Bio_en { get; set; }
        public string? ImageUrl { get; set; }
        public CountryCode CountryCode { get; set; } = CountryCode.EG;
        public bool IsActive { get; set; }
        public virtual ICollection<Book> Books { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];

        public virtual ICollection<Quote> Quotes { get; set; } = [];

        public virtual ICollection<UserFollow> Followers { get; set; } = [];
    }
}