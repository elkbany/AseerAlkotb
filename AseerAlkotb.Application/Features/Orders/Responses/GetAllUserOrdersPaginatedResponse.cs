

using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record GetAllUserOrdersPaginatedResponse
    (
       int OrderId,
       string UserName,
       PaymentMethod PaymentMethod,
       PaymentStatus PaymentStatus,
       EgyptGovernorates Governorate,
       OrderStatus OrderStatus,
       string TrackingNumber,
       decimal TotalAmount,
       DateTime OrderDate,
       List<BookDTO> Books
    );
}
