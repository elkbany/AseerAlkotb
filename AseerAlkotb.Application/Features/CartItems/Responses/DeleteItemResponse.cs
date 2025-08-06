using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.CartItems.Responses
{
    public record DeleteItemResponse(int CartId, int BookId);
    
}
