using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Features.Wishlist.Responses
{
    public record GetUserWishlistResponse(int UserId, IEnumerable<WishlistItem> Items);

}
