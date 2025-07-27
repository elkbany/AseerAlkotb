

namespace AseerAlkotb.Domain.Entites
{
    public class Book : Entity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice => Price - (Price * DiscountPercentage / 100);

        public DateTime PublishedDate { get; set; }
        public int PageCount { get; set; }
        public string Language { get; set; }
        public string CoverImageUrl { get; set; }
        public string Format { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int SalesCount { get; set; }

        // Navigation properties
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    
    }
}
