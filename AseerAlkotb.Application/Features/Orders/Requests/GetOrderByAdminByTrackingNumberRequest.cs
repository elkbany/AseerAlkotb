using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Orders.Requests
{
   public record GetOrderByAdminByTrackingNumberRequest
    (
     string TrackingNumber
    );
}
