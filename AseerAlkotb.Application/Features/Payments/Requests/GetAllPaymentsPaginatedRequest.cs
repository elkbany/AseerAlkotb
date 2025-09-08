using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Payments.Requests
{
    public record GetAllPaymentsPaginatedRequest(
        PaymentStatus? Status = null,
        PaymentMethod? PaymentMethod = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        string? CustomerSearch = null,
        bool DateAscending = false,
        int PageNumber = 1,
        int PageSize = 10,
        string Search = ""
    );
}