using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Requests
{
   public record GetAllReviewsPaginatedRequest
    (
        int? AuthorId ,
        int? BookId,
        int PageNumber=1,
        int PageSize=10,
        string Search = ""
    );
}
