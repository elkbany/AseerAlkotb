using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Mapping
{
    public record AddBookResponse(
        int Id,
        string Title,
        string ISBN,
        string Description,
        decimal Price,
        decimal DiscountPercentage,
        decimal DiscountedPrice,
        DateTime PublishedDate,
        int PageCount,
        BookLanguage Language,
        string Format,
        string CoverImageUrl,
        int StockQuantity,
        string AuthorName,       
        string PublisherName,    
        List<string> CategoryNames,
        bool IsActive
        );
}
