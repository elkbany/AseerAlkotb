using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record UpdatePaymentStatusRequest(
        int PaymentId,
        PaymentStatus NewStatus,
        string? Notes = null
    );
}