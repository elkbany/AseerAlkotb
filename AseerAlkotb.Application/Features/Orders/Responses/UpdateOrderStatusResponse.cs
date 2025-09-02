using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record UpdateOrderStatusResponse(
        int Id,
        OrderStatus Status,
        string TrackingNumber,
        DateTime UpdatedAt
    );
}