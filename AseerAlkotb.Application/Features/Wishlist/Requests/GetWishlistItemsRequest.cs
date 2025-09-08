using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Wishlist.Requests
{
    public record GetWishlistItemsRequest(int PageNumber=1,int PageSize=5);
   
}
