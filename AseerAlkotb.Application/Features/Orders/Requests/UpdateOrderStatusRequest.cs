using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record UpdateOrderStatusRequest(
        int OrderId,
        OrderStatus NewStatus
    );
}