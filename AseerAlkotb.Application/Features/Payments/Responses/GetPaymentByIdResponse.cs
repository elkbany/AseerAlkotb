using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public record GetPaymentByIdResponse(
        int Id,
        string TransactionId,
        int OrderId,
        int UserId,
        string CustomerName,
        string CustomerEmail,
        string CustomerPhone,
        PaymentMethod PaymentMethod,
        PaymentStatus Status,
        decimal Amount,
        string Currency,
        DateTime PaymentDate,
        long? PaymobOrderId = null,
        string? AdminNotes = null
    );
}