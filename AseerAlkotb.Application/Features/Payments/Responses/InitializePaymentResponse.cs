using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public record InitializePaymentResponse(
        int PaymentId,
        string TransactionId,
        PaymentMethod PaymentMethod,
        decimal Amount,
        string Currency,
        PaymentStatus Status,
        string? RedirectUrl = null,
        string? Instructions = null,
        bool RequiresRedirect = false
    );
}