using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Rag.Responses
{
    public record BookBriefDto(
        int Id,
        string Title,
        string? AuthorName,
        decimal Price,
        decimal DiscountedPrice,
        string? CoverImageUrl,
        string? Description   
    );

}
