

using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Requests
{
  public record GetAllOrdersPaginatedRequest
   (
      OrderStatus? OrderStatus,
      EgyptGovernorates? Governorate,
      bool DateAscending =true,
      int PageNumber=1,
      int PageSize=10,
      string Search=""
   );
}
