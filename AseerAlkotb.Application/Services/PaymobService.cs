﻿﻿using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public PaymobService(IConfiguration configuration, IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }

        public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            var Order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (Order == null)
            {
                throw new Exception("Order not found");
            }

            //var User = await _unitOfWork.Users.GetByIdAsync(Order.UserId);
            //if (User == null)
            //{
            //    throw new Exception("User not found");
            //}

            string apiKey = _configuration["Paymob:ApiKey"] ?? throw new Exception("Paymob API Key not configured");
            string secretKey = _configuration["Paymob:SecretKey"] ?? throw new Exception("Paymob Secret Key not configured");
            string publicKey = _configuration["Paymob:PublicKey"] ?? throw new Exception("Paymob Public Key not configured");

            // Generate a special Reference for this Transaction
            int specialReference = $"{Order.Id}{Order.UserId}{DateTime.UtcNow.Ticks}".GetHashCode() & 0x7FFFFFFF; // Ensure positive hash code

            // Create intention request Payload
            var amountCents = (int)(Order.TotalAmount * 100); // Convert to cents

            var billingData = new
            {

                first_name = Order.User.FirstName ?? "Guest",
                last_name = Order.User.LastName ?? "User",
                email = Order.User.Email,
                phone_number = "+2011282928979",
                street = "N/A",
                building = "N/A",
                city = Order.Governorate,
                country = "Egypt",
                floor = "N/A",
                state = "N/A"
            };

            // Get Wallet Integration Id 
            var IntegrationId = int.Parse(DetermineIntegrationId(request.PaymentMethod));

            var payload = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = new[] { IntegrationId },
                billing_data = billingData,
                items = new[]
                {
                    new {
                        name = $"Order #{specialReference-145}",
                        amount = amountCents,
                        description = "Payment for Order: " + Order.Id,
                        quantity = 1
                    }
                },
                customer = new
                {
                    first_name = Order.User.FirstName ?? "Guest",
                    last_name = Order.User.LastName ?? "User",
                    email = Order.User.Email,
                    //phone_number = Order.User.PhoneNumber
                },
                extras = new
                {
                    orderId = Order.Id,
                    customerId = Order.User.Id
                },
                special_reference = specialReference,
                expiration = 3600, // 1 hour in seconds
            };

            // Create Http Request for Paymob's Intention API
            var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", secretKey);
            requestMessage.Content = JsonContent.Create(payload);

            // Send the Request and process the Response
            var response = await _httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paymob API Error: {response.StatusCode}: {responseContent}");
            }

            // Parse the Response to get the client_secret
            var resultJson = JsonDocument.Parse(responseContent);
            var clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString() 
                ?? throw new Exception("Client secret not found in Paymob response");

            // Create the payment record in our database
            var payment = new Payment
                {
                Amount = Order.TotalAmount,
                Method = request.PaymentMethod.ToLower() == "card" ? PaymentMethod.Card : PaymentMethod.MobileWallet,
                UserId = Order.UserId,
                OrderId = Order.Id,
                Status = PaymentStatus.Pending,
                TransactionId = specialReference.ToString(),
                PaymentDate = DateTime.UtcNow,
                ProviderPayload = responseContent
                };


            await _unitOfWork.Payments.InsertAsync(payment);
            Order.PaymentStatus = PaymentStatus.Pending;
            await _unitOfWork.CommitAsync();

            // Generate Payment URL for the Unified Checkout
            string redirectUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            return new ProcessPaymentResponse
            {
                RedirectUrl = redirectUrl
            };

        }

        public async Task<Order> UpdateOrderSuccess(string specialReference)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == specialReference);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
            }

            var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with Order Id {payment.OrderId} not found");
            }

            // Update Order Status and Payment Status
            order.PaymentStatus = PaymentStatus.Paid;
            payment.Status = PaymentStatus.Paid;

            // TODO: Add notification functionality when Notifications table is created
            /*
            var notification = new Notification
            {
                Title = "Order Purchased Successfully ",
                Message = $"You have Successfully Payment for Your Order",
                OrderId = order.Id,
                NotificationType = NotificationTypes.PaymentSuccess,
                UserId = order.User.Id
            };

            await _unitOfWork.Notifications.InsertAsync(notification);
            */

            await _unitOfWork.CommitAsync();

            return payment.Order;
        }

        public async Task<Order> UpdateOrderFailed(string specialReference)
        {
            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(p => p.TransactionId == specialReference);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
            }

            var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with Order Id {payment.OrderId} not found");
            }

            // Update Order Status and Payment Status
            order.PaymentStatus = PaymentStatus.Failed;
            payment.Status = PaymentStatus.Failed;

            // TODO: Add notification functionality when Notifications table is created
            /*
            var notification = new Notification
            {
                Title = "Order Purchased Failed ",
                Message = $"You have Failed Payment for Your Order",
                OrderId = order.Id,
                NotificationType = NotificationTypes.PaymentSuccess,
                UserId = order.User.Id
            };

            await _unitOfWork.Notifications.InsertAsync(notification);
            */

            await _unitOfWork.CommitAsync();

            return payment.Order;
        }

        private string DetermineIntegrationId(string paymentMethod)
        {
            return paymentMethod.ToLower() switch
            {
                "card" => _configuration["Paymob:CardIntegrationId"] ?? throw new Exception("Paymob Card Integration ID not configured"),
                "wallet" => _configuration["Paymob:WalletIntegrationId"] ?? throw new Exception("Paymob Wallet Integration ID not configured"),
                _ => throw new ArgumentException("Invalid payment method specified")
            };
        }

        public string ComputeHmacSHA512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

        }
    }
}
