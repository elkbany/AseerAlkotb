using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Wishlist.Responses
{
    public record WishlistItemResponse(
    int BookId,
    string Title,
    string Description,
    decimal Price,
    string AuthorName,
    string CoverImageUrl
);
}
