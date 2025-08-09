using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.CartItems.Requests
{
    public record DeleteItemRequest(int UserId, int bookId);
   
}
