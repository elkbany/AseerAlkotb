

namespace AseerAlkotb.Application.Features.Reviews.Requests
{
  public record AddReviewRequest
        (
        int? AuthorId,
        int? BookId,
        int Rating,
        string? Comment
    );

}
