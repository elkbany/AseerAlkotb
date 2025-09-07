﻿﻿﻿

using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record AddOrderResponse
    (
        int Id,
        string TrackingNumber,
        PaymentInitializationInfo? PaymentInfo = null
    );

    public record PaymentInitializationInfo
    (
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
