
namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record GetAllUserOrdersPaginatedRequest
    (
      int UserId,
      int PageNumber = 1,
      int PageSize = 10,
      string Search = ""
    );
}
