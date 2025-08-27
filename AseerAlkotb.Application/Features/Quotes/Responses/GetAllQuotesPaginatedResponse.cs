using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Quotes.Responses
{
    public record GetAllQuotesPaginatedResponse(int Id , int? AuthorId , int? BookId , int UserId , string Comment);

}
