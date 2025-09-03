﻿﻿using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Orders.Responses
{
    public record GetOrderByAdminByTrackingNumberResponse
    (
        int Id,
       string UserName,
       PaymentMethod PaymentMethod,
       PaymentStatus PaymentStatus,
       EgyptGovernorates Governorate,
        EgyptCities City,
       OrderStatus OrderStatus,
       string TrackingNumber,
       decimal TotalAmount,
       decimal ShippingCost,
       decimal TaxAmount,
       decimal DiscountAmount,
       decimal FinalAmount,
       DateTime OrderDate,
       DateTime UpdatedAt,
       List<BookDTO> Books
    );
}
