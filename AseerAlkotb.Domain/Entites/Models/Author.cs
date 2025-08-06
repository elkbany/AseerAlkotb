using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Author : Entity<int>
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Book> Books { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];
    }
}
