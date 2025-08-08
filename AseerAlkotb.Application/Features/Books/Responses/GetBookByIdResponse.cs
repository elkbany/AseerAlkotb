using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Responses
{
    public record GetBookByIdResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime PublishedDate { get; set; }
        public int PageCount { get; set; }
        public BookLanguage Language { get; set; }
        public string? CoverImageUrl { get; set; }
        public string Format { get; set; }
        public int StockQuantity { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<string> CategoryNames { get; set; }
        public bool IsActive { get; set; }
        public decimal Rating { get; set; } // now mutable
        public string Comment { get; set; }
    }

}
