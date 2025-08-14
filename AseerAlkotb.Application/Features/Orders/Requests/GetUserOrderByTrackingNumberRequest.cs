

namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record GetUserOrderByTrackingNumberRequest
    (
        int UserId,
        string TrackingNumber
    );
}
