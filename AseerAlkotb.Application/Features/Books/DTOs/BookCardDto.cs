using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.DTOs
{
    public record BookCardDto
    (
        int Id ,
        string Title,
        decimal Price,
        decimal DiscountedPrice,
        string CoverImageUrl,
        string AuthorName,
        int Rating
    );
    
}
