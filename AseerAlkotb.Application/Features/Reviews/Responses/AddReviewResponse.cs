

namespace AseerAlkotb.Application.Features.Reviews.Responses
{
     public record AddReviewResponse
     (
       int Id,
       int? AuthorId,
       int? BookId,
       int UserId,
       int Rating,
       string Comment 
     );
}

