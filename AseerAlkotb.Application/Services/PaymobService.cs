﻿﻿﻿﻿﻿﻿﻿using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using AseerAlkotb.Domain.Interfaces.Base;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Entites;

namespace AseerAlkotb.Application.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymobService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;

        public PaymobService(
            IConfiguration configuration,
            ILogger<PaymobService> logger,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpClient = httpClient;
        }

        public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            // Load Order with navigation properties
            var Order = await _unitOfWork.Orders.FirstOrDefaultAsync(
                o => o.Id == request.OrderId,
                default,
                o => o.User, o => o.OrderItems
            );

            if (Order == null)
            {
                throw new Exception("Order not found");
            }

            if (!Order.OrderItems.Any())
            {
                throw new Exception("Order has no items");
            }

            string apiKey = _configuration["Paymob:ApiKey"] ?? throw new Exception("Paymob API Key not configured");
            string secretKey = _configuration["Paymob:SecretKey"] ?? throw new Exception("Paymob Secret Key not configured");
            string publicKey = _configuration["Paymob:PublicKey"] ?? throw new Exception("Paymob Public Key not configured");

            // Generate a special Reference for this Transaction
            int specialReference = $"{Order.Id}{Order.UserId}{DateTime.UtcNow.Ticks}".GetHashCode() & 0x7FFFFFFF; // Ensure positive hash code

            // Create intention request Payload
            var amountCents = (int)(Order.FinalAmount * 100); // Convert to cents

            var billingData = new
            {
                first_name = Order.FirstName ?? "Guest",
                last_name = Order.LastName ?? "User",
                email = Order.User.Email,
                phone_number = Order.PhoneNumber ?? "+201128292897",
                street = Order.StreetAddress ?? "N/A",
                building = "N/A",
                city = Order.City,
                country = "Egypt",
                floor = "N/A",
                state = Order.Governorate.ToString() ?? "N/A"
            };

            // Get Wallet Integration Id 
            var IntegrationId = int.Parse(DetermineIntegrationId(request.PaymentMethod));

            var payload = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = new[] { IntegrationId },
                billing_data = billingData,
                // add order items = list of items in the order with proper structure
                //items = Order.OrderItems.Select(item => new
                //{
                //    name = $"Book ID: {item.BookId}",
                //    amount = (int)(item.UnitPrice * 100), // amount in cents
                //    description = $"Book from Order #{Order.Id}",
                //    quantity = item.Quantity
                //}).ToList(),

                items = new[]
                {
                    new {
                        name = $"Order #{specialReference-145}",
                        amount = amountCents,
                        description = "Payment for Order: " + Order.Id,
                    }
                },
                customer = new
                {
                    first_name = Order.FirstName ?? "Guest",
                    last_name = Order.LastName ?? "User",
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

            _logger.LogInformation("Sending payment request to Paymob API for Order {OrderId}, Amount: {Amount}, Special Reference: {SpecialReference}",
                Order.Id, amountCents, specialReference);

            // Send the Request and process the Response with timeout and error handling
            HttpResponseMessage response;
            string responseContent;

            try
            {
                // Add longer timeout for network issues
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                response = await _httpClient.SendAsync(requestMessage, cts.Token);
                responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Received response from Paymob API - Status: {StatusCode}, Content Length: {ContentLength}",
                    response.StatusCode, responseContent?.Length ?? 0);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Paymob API request timed out after 60 seconds for Order {OrderId}", Order.Id);
                throw new TimeoutException("Paymob API request timed out after 60 seconds");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error communicating with Paymob API for Order {OrderId}. Error: {ErrorMessage}", Order.Id, httpEx.Message);

                // Check for specific network issues
                if (httpEx.Message.Contains("No such host is known"))
                {
                    throw new InvalidOperationException("DNS resolution failed for Paymob API. Please check your internet connection and DNS settings.", httpEx);
                }

                throw new InvalidOperationException($"HTTP error communicating with Paymob API: {httpEx.Message}", httpEx);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Paymob API Error: {response.StatusCode}: {responseContent}");
            }

            // Parse the Response to get the client_secret with error handling
            JsonDocument resultJson;
            string clientSecret;

            try
            {
                resultJson = JsonDocument.Parse(responseContent);
                clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString()
                    ?? throw new InvalidOperationException("Client secret is null in Paymob response");
            }
            catch (JsonException jsonEx)
            {
                throw new InvalidOperationException($"Failed to parse Paymob API response: {jsonEx.Message}. Response: {responseContent}", jsonEx);
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException($"Client secret not found in Paymob response. Response: {responseContent}");
            }

            // Create the payment record in our database
            var payment = new Payment
            {
                Amount = Order.TotalAmount,
                Method = request.PaymentMethod.ToLower() == "card" ? PaymentMethod.Card : PaymentMethod.Wallet,
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
            try
            {
                if (string.IsNullOrEmpty(specialReference))
                {
                    throw new ArgumentException("Special reference cannot be null or empty", nameof(specialReference));
                }

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

                // Validate current status before updating
                if (payment.Status == PaymentStatus.Paid)
                {
                    // Already processed, return order without error
                    return order;
                }

                if (payment.Status == PaymentStatus.Failed)
                {
                    throw new InvalidOperationException($"Cannot mark failed payment {specialReference} as successful");
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

                return order;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException || ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Unexpected error updating order success for reference {specialReference}: {ex.Message}", ex);
            }
        }

        public async Task<Order> UpdateOrderFailed(string specialReference)
        {
            try
            {
                if (string.IsNullOrEmpty(specialReference))
                {
                    throw new ArgumentException("Special reference cannot be null or empty", nameof(specialReference));
                }

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

                // Validate current status before updating
                if (payment.Status == PaymentStatus.Failed)
                {
                    // Already processed, return order without error
                    return order;
                }

                if (payment.Status == PaymentStatus.Paid)
                {
                    throw new InvalidOperationException($"Cannot mark successful payment {specialReference} as failed");
                }

                // Update Order Status and Payment Status
                order.PaymentStatus = PaymentStatus.Failed;
                payment.Status = PaymentStatus.Failed;

                // TODO: Add notification functionality when Notifications table is created
                /*
                var notification = new Notification
                {
                    Title = "Order Payment Failed ",
                    Message = $"Payment for your order has failed. Please try again.",
                    OrderId = order.Id,
                    NotificationType = NotificationTypes.PaymentFailed,
                    UserId = order.User.Id
                };

                await _unitOfWork.Notifications.InsertAsync(notification);
                */

                await _unitOfWork.CommitAsync();

                return order;
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is KeyNotFoundException || ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"Unexpected error updating order failure for reference {specialReference}: {ex.Message}", ex);
            }
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

        public bool ValidateWebhookHmac(PaymentWebhookData webhookData, string receivedHmac, string hmacSecret)
        {
            try
            {
                // For webhooks, we use the same field concatenation method as callbacks
                // According to Paymob documentation, the HMAC concatenation should be exactly in this order:
                // amount_cents + created_at + currency + error_occured + has_parent_transaction + id + integration_id + is_3d_secure + is_auth + is_capture + is_refunded + is_standalone_payment + is_voided + order + owner + pending + source_data.pan + source_data.sub_type + source_data.type + success

                // Important: All boolean values should be lowercase strings ("true" or "false")
                string FormatBooleanValue(bool value)
                {
                    return value.ToString().ToLowerInvariant();
                }

                string FormatNullableBooleanValue(bool? value)
                {
                    return value.HasValue ? value.Value.ToString().ToLowerInvariant() : "";
                }

                var fields = new[]
                {
                    webhookData.Obj.AmountCents.ToString(),                    // amount_cents: 0
                    webhookData.Obj.CreatedAt ?? "",                           // created_at: 1
                    webhookData.Obj.Currency ?? "",                            // currency: 2
                    FormatBooleanValue(webhookData.Obj.ErrorOccured),          // error_occured: 3
                    FormatNullableBooleanValue(webhookData.Obj.HasParentTransaction), // has_parent_transaction: 4
                    webhookData.Obj.Id.ToString(),                             // id: 5
                    webhookData.Obj.IntegrationId ?? "",                       // integration_id: 6
                    FormatNullableBooleanValue(webhookData.Obj.Is3dSecure),   // is_3d_secure: 7
                    FormatNullableBooleanValue(webhookData.Obj.IsAuth),        // is_auth: 8
                    FormatNullableBooleanValue(webhookData.Obj.IsCapture),     // is_capture: 9
                    FormatNullableBooleanValue(webhookData.Obj.IsRefunded),   // is_refunded: 10
                    FormatNullableBooleanValue(webhookData.Obj.IsStandalonePayment), // is_standalone_payment: 11
                    FormatNullableBooleanValue(webhookData.Obj.IsVoided),      // is_voided: 12
                    webhookData.Obj.Order.Id.ToString(),                       // order: 13
                    webhookData.Obj.Owner ?? "",                              // owner: 14
                    FormatBooleanValue(webhookData.Obj.Pending),               // pending: 15
                    webhookData.Obj.SourceData?.Pan ?? "",                     // source_data.pan: 16
                    webhookData.Obj.SourceData?.SubType ?? "",                 // source_data.sub_type: 17
                    webhookData.Obj.SourceData?.Type ?? "",                    // source_data.type: 18
                    FormatBooleanValue(webhookData.Obj.Success)                // success: 19
                };

                var concatenated = string.Join("", fields);
                var calculatedHmac = ComputeHmacSHA512(concatenated, hmacSecret);

                _logger.LogInformation("Webhook HMAC Validation - Concatenated String: {Concatenated}", concatenated);
                _logger.LogInformation("Webhook HMAC Validation - Concatenated Length: {Length}", concatenated.Length);
                _logger.LogInformation("Webhook HMAC Validation - Calculated: {Calculated}, Received: {Received}",
                    calculatedHmac, receivedHmac);

                var isValid = CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(receivedHmac ?? ""),
                    Encoding.UTF8.GetBytes(calculatedHmac)
                );
                
                _logger.LogInformation("Webhook HMAC Validation Result: {IsValid}", isValid ? "Valid ✅" : "Invalid ❌");

                if (!isValid)
                {
                    _logger.LogWarning("Webhook HMAC Validation Details - Expected: {Expected}, Got: {Received}, String Length: {Length}",
                        calculatedHmac, receivedHmac, concatenated.Length);
                    
                    // Additional debugging for field-by-field analysis
                    _logger.LogWarning("Webhook HMAC Field Values:");
                    string[] fieldNames = {
                        "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                        "id", "integration_id", "is_3d_secure", "is_auth", "is_capture",
                        "is_refunded", "is_standalone_payment", "is_voided", "order", "owner",
                        "pending", "source_data.pan", "source_data.sub_type", "source_data.type", "success"
                    };
                    
                    for (int i = 0; i < fields.Length; i++)
                    {
                        _logger.LogWarning("  {FieldName} ({Index}): '{Value}'", fieldNames[i], i, fields[i]);
                    }
                    
                    _logger.LogWarning("  Full Concatenated String: '{Concatenated}'", concatenated);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating webhook HMAC using field concatenation");
                return false;
            }
        }
    }
}