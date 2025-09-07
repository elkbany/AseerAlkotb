using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Responses
{
    public record GetAllPaymentsPaginatedResponse(
        int Id,
        string TransactionId,
        int OrderId,
        string CustomerName,
        string CustomerEmail,
        PaymentMethod PaymentMethod,
        PaymentStatus Status,
        decimal Amount,
        string Currency,
        DateTime PaymentDate,
        long? PaymobOrderId = null
    );
}