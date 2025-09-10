﻿using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IPaymobService
    {
        public Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request);
        public Task<Order> UpdateOrderSuccess(string specialReference);
        public Task<Order> UpdateOrderFailed(string specialReference);
        public string ComputeHmacSHA512(string data, string secret);
        //public bool ValidateWebhookHmac(string body, string receivedHmac, string hmacSecret);
        public bool ValidateWebhookHmac(PaymentWebhookData webhookData, string receivedHmac, string hmacSecret);
    }
}