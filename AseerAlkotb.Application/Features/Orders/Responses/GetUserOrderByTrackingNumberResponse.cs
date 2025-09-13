﻿using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record GetUserOrderByTrackingNumberResponse
    (
        int Id,
       string UserName,
       int UserId,
       PaymentMethod PaymentMethod,
       PaymentStatus PaymentStatus,
       int GovernorateId,
       string GovernorateName,
       int CityId,
       string CityName,
       OrderStatus OrderStatus,
       string TrackingNumber,
       decimal FinalAmount,
       decimal ShippingCost,
       decimal DiscountedAmount,
       decimal TotalAmount,
       DateTime OrderDate,
       List<BookDTO> Books,
       string PhoneNumber,//added
       string StreetAddress,//added
       string FirstName,//added
       string LastName,//added
       string email//added

    );
}
