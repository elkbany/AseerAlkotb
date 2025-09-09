﻿
using AseerAlkotb.Domain.Enums;


namespace AseerAlkotb.Application.Features.Orders.Requests
{
    public record AddOrderRequest
        (
        String FirstName,
        String LastName,
        String StreetAddress,
        String PhoneNumber,
        int GovernorateId,
        int CityId,
        PaymentMethod PaymentMethod
        );
  
}
