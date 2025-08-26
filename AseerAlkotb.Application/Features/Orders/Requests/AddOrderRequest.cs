
using AseerAlkotb.Domain.Enums;


namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record AddOrderRequest
        (
        EgyptGovernorates Governorate,
        EgyptCities City,
        PaymentMethod PaymentMethod,
        int UserId
        );
  
}
