namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record PaymentCallbackRequest(
        string TransactionId,
        string Success,
        string AmountCents,
        string CreatedAt,
        string Currency,
        string ErrorOccured,
        string HasParentTransaction,
        string Id,
        string IntegrationId,
        string Is3dSecure,
        string IsAuth,
        string IsCapture,
        string IsRefunded,
        string IsStandalonePayment,
        string IsVoided,
        string Order,
        string Owner,
        string Pending,
        string SourceDataPan,
        string SourceDataSubType,
        string SourceDataType,
        string MerchantOrderId,
        string Hmac
    );
}