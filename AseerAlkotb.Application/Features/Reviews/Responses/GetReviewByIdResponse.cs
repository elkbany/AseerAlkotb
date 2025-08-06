

namespace AseerAlkotb.Application.Features.Reviews.Responses
{
    public record GetReviewByIdResponse
    (
      int Id,
      int? AuthorId,
      int? BookId,
      int UserId,
      int Rating,
      string Comment
    );
}
