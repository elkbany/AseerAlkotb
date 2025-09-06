using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Roles.Responses
{
    public class UserDetailsResponse
    {
        public GetAllAdminResponse User { get; set; }
        public List<OrderDetailsResponse> Orders { get; set; } = new();
        public List<ReviewDetailsResponse> Reviews { get; set; } = new();
        public List<QuoteDetailsResponse> Quotes { get; set; } = new();
        public List<WishlistItemDetailsResponse> Wishlist { get; set; } = new();
    }

    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        public List<OrderItemDetailsResponse> OrderItems { get; set; } = new();
    }

    public class OrderItemDetailsResponse
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImageUrl { get; set; }
        public string AuthorName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ReviewDetailsResponse
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImageUrl { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class QuoteDetailsResponse
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImageUrl { get; set; }
        public string QuoteText { get; set; }
        public int PageNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WishlistDetailsResponse
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImageUrl { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class WishlistItemDetailsResponse
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImageUrl { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
