

namespace AseerAlkotb.Application.Features.Books.DTOs
{
    public record BookCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string CoverImageUrl { get; set; }
        public string AuthorName { get; set; }
        public decimal Rating { get; set; } // now mutable
        public string Comment { get; set; }
    }

}
