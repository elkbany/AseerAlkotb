using AseerAlkotb.Domain.Entites.Base;

namespace AseerAlkotb.Domain.Entites.Models
{
    public class Book : Entity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice => Price - Price * DiscountPercentage / 100;

        public DateTime PublishedDate { get; set; }
        public int PageCount { get; set; }
        public string Language { get; set; }
        public string CoverImageUrl { get; set; }
        public string Format { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int SalesCount { get; set; }

       
        #region Navigation Properties
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public int PublisherId { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
        public virtual ICollection<CartItem> CartItems { get; set; } = []; 
        public virtual ICollection<Wishlist> Wishlists { get; set; } = [];
        #endregion


    }
}
