

namespace AseerAlkotb.Application.Features.Reviews.Responses
{
  public record GetAllReviewsPaginatedResponse
   (
      int Id,
      int? BookId,
      int? AuthorId,
      int UserId,
      int Rating,
      string Comment
   );
}

