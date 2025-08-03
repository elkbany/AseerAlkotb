using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Responses
{
    public record UpdateBookResponse(
        int Id,
        string Title,
        string ISBN,
        decimal Price,
        string Description,
        decimal DiscountPercentage,
        DateTime PublishedDate,
        int PageCount,
        BookLanguage Language,
        string? CoverImageUrl,
        string Format,
        int StockQuantity,
        int AuthorId,
        int PublisherId,
        List<int> CategoryIds,
        bool IsActive
    );
}
