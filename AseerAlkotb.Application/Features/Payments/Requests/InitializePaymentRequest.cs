using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record InitializePaymentRequest(
        int OrderId,
        PaymentMethod PaymentMethod,
        int UserId
    );
}