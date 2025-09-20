﻿﻿﻿﻿using AseerAlkotb.Application.Features.Books.DTOs;
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
       int GovernorateId,
       string GovernorateName,
       int CityId,
       string CityName,
       OrderStatus OrderStatus,
       string TrackingNumber,
       decimal TotalAmount,
       decimal ShippingCost,
       decimal DiscountAmount,
       decimal FinalAmount,
       DateTime OrderDate,
       DateTime UpdatedAt,
       int Quantity,
       List<BookDTO> Books
    );
}
