using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Requests
{
    public record UpdateBookRequest(
        int Id,
        string Title,
        string Description,
        string ISBN,
        decimal Price,
        decimal DiscountPercentage,
        DateTime PublishedDate,
        int PageCount,
        BookLanguage Language,
        IFormFile? CoverImageUrl,
        string Format,
        int StockQuantity,
        int AuthorId,
        int PublisherId,
        List<int> CategoryIds,
        bool IsActive
    );
}
