﻿﻿﻿﻿using AseerAlkotb.API.Bases;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Services;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : AppControllerBase
    {
        private readonly IOrderServices orderServices;
        private readonly IUnitOfWork unitOfWork;

        public OrdersController(IOrderServices orderServices, IUnitOfWork unitOfWork)
        {
            this.orderServices = orderServices;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromQuery] AddOrderRequest request)
        {
            var result = await orderServices.CheckoutAsync(request);
            return ApiResult(result);
        }

        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel([FromQuery] CancelOrderRequest request)
        {
            var result = await orderServices.CancelOrderAsync(request);
            return ApiResult(result);
        }

        [HttpGet("Admin/GetAll")]
        public async Task<IActionResult> GetAllOrdersByAdmin([FromQuery] GetAllOrdersPaginatedRequest request)
        {
            var result = await orderServices.GetAllOrdersPaginatedByAdminAsync(request);
            return ApiResult(result);
        }

        [HttpGet("User/GetAll")]
        public async Task<IActionResult> GetAllOrdersByUser([FromQuery] GetAllUserOrdersPaginatedRequest request)
        {
            var result = await orderServices.GetAllUserOrdersPaginatedAsync(request);
            return ApiResult(result);
        }

        [HttpGet("Admin/GetByTrackingNumber")]
        public async Task<IActionResult> GetByTrackingNumberByAdmin([FromQuery] GetOrderByAdminByTrackingNumberRequest request)
        {
            var result = await orderServices.GetOrderByTrackingNumberByAdminAsync(request);
            return ApiResult(result);
        }

        [HttpGet("User/GetByTrackingNumber")]
        public async Task<IActionResult> GetByTrackingNumberByUser([FromQuery] GetUserOrderByTrackingNumberRequest request)
        {
            var result = await orderServices.GetOrderByTrackingNumberByUserAsync(request);
            return ApiResult(result);
        }
        [HttpGet("GetShippingCost")]
        public async Task<decimal> GetShippingCost([FromQuery] int governorateId)
        {
             var result = await ShippingServices.GetShippingCostForAGovernorateAsync(governorateId, unitOfWork);
             return result;
        }
    }
}
