

using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record GetAllUserOrdersPaginatedResponse
    (
       int Id,
       string UserName,
       PaymentMethod PaymentMethod,
       PaymentStatus PaymentStatus,
       EgyptGovernorates Governorate,
         EgyptCities City,
       OrderStatus OrderStatus,
       string TrackingNumber,
       decimal FinalAmount,
       DateTime OrderDate,
       List<BookDTO> Books
    );
}
