using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Books.Responses
{
    public record UpdateBookResponse(
        int Id,
        string Title,
        string Title_en,
        string ISBN,
        decimal Price,
        string Description,
        string Description_en,
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
