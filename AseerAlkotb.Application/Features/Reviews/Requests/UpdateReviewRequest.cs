

namespace AseerAlkotb.Application.Features.Reviews.Requests
{
    public record UpdateReviewRequest
   (
        int Id,
        int Rating,
        string Comment
    );
    
}
