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
        decimal UnitPrice,
        int Quantity,
        decimal TotalPrice
    );
    
}
