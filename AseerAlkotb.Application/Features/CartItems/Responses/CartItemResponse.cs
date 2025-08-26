using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.CartItem.Responses
{
    public record CartItemResponse
    (
        //int Id,
        int BookId,
        string BookTitle,
        string CoverImageUrl,
        decimal UnitPrice,
        decimal DiscountPercentage,
        decimal DiscountedPrice,
        int Quantity,
        decimal TotalPrice,
        decimal TotalDiscountedPrice 

    );
    
}
