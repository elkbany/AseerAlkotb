using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Services
{
    public class PaymentService : AppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;   
        private readonly HttpClient _httpClient;   
        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config, HttpClient httpClient, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment): base(serviceProvider, hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpClient = httpClient;
        }

        //public async Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request)
        //{
        //    var Orders = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        //    if (Orders == null)
        //    {
        //        return new InitiatePaymentResponse("s",true, "Sucessf", "s");
        //    }

            

        //}

        public Task ProcessCallbackAsync(PaymentCallbackRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
