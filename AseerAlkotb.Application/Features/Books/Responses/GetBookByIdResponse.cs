using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.Responses
{
    public record GetBookByIdResponse(
    int Id,
    string Title,
    string Description,
    string ISBN,
    decimal Price,
    decimal DiscountPercentage,
    DateTime PublishedDate,
    int PageCount,
    string Language,
    string? CoverImageUrl,
    string Format,
    int StockQuantity,
    int AuthorId,
    string AuthorName,
    int PublisherId,
    string PublisherName,
    List<int> CategoryIds,
    List<string> CategoryNames,
    bool IsActive
    );
}
