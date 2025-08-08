using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Reviews.Requests
{
  public record AddReviewRequest
        (
        int? AuthorId,
        int? BookId,
        int UserId,
        int Rating,
        string Comment
    );

}
