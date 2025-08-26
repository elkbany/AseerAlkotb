using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Requests
{
    public record UpdateQuoteRequest(
        int Id,
        string Comment
    );

}
