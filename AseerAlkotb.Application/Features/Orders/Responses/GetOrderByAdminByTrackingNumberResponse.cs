using AseerAlkotb.Application.Features.Books.DTOs;
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
