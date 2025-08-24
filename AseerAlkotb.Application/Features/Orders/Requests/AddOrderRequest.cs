using AseerAlkotb.Application.Features.OrderItems.DTOs;
using AseerAlkotb.Domain.Enums;


namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record AddOrderRequest
        (
        EgyptGovernorates Governorate,
        PaymentMethod PaymentMethod,
        int UserId
        );
  
}
