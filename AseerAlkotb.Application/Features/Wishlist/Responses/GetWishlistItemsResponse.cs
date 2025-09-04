using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Wishlist.Responses
{
    public record GetWishlistItemsResponse(
        string BookName,
        int BookId, 
        string AutherName,
        int AutherId, 
        string ImageUrl,
        decimal Price,
        decimal DiscountedPrice

        );

}
