
namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record GetAllUserOrdersPaginatedRequest
    (
      int PageNumber = 1,
      int PageSize = 10,
      string Search = ""
    );
}
