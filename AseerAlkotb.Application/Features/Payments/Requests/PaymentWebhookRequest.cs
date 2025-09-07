using System.Text.Json.Serialization;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    /// <summary>
    /// Paymob webhook data structure
    /// </summary>
    public record PaymentWebhookData(
        string Type,
        PaymentWebhookTransaction Obj
    );

    public record PaymentWebhookTransaction(
        long Id,
        bool Success,
        int AmountCents,
        bool Pending,
        PaymentWebhookOrder Order,
        string CreatedAt,
        string Currency,
        PaymentWebhookSourceData? SourceData,
        bool ErrorOccured
    );

    public record PaymentWebhookOrder( 
        long Id,
        string? MerchantOrderId,
        int AmountCents,
        string Currency,
        string PaymentStatus
    );

    public record PaymentWebhookSourceData(
        string? Pan,
        string? Type,
        string? SubType,
        string? PhoneNumber
    );
}