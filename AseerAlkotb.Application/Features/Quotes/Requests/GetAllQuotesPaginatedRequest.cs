using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Requests
{
    public record GetAllQuotesPaginatedRequest(
        int? AuthorId,
        int? BookId ,
        string SearchTerm = "",
        int PageNumber=1,
        int PageSize=10
    );

}
