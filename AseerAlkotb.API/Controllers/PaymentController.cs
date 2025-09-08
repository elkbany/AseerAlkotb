﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using AseerAlkotb.API.Helpers;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            IPaymobService paymobService, 
            IConfiguration configuration,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _paymobService = paymobService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Initialize payment for an order
        /// </summary>
        /// <param name="request">Payment initialization request</param>
        /// <returns>Payment initialization response with redirect URL or confirmation</returns>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequest request)
        {
            try
            {


                var response = await _paymentService.InitializePaymentAsync(request);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Payment initialized successfully for Order {OrderId}", request.order.Id);
                    return Ok(response);
                }
                
                _logger.LogWarning("Payment initialization failed for Order {OrderId}: {Message}", 
                    request.order.Id, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment for Order {OrderId}", request.order.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    BadRequest<string>("An error occurred while initializing payment"));
            }
        }

        /// <summary>
        /// Get paginated list of payments for Angular dashboard
        /// </summary>
        /// <param name="request">Pagination and filter request</param>
        /// <returns>Paginated payments list</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetPayments([FromQuery] GetAllPaymentsPaginatedRequest request)
        {
            try
            {
                _logger.LogInformation("Fetching payments - Page {PageNumber}, Size {PageSize}", 
                    request.PageNumber, request.PageSize);

                var response = await _paymentService.GetAllPaymentsPaginatedAsync(request);
                
                if (response.Succeeded)
                {
                    return Ok(response);
                }
                
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payments");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    BadRequest<string>("An error occurred while fetching payments"));
            }
        }

        /// <summary>
        /// Get payment details by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching payment details for Payment {PaymentId}", id);

                var response = await _paymentService.GetPaymentByIdAsync(id);
                
                if (response.Succeeded)
                {
                    return Ok(response);
                }
                
                return NotFound(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment {PaymentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    BadRequest<string>("An error occurred while fetching payment details"));
            }
        }

        /// <summary>
        /// Update payment status (Admin only)
        /// </summary>
        /// <param name="request">Status update request</param>
        /// <returns>Update result</returns>
        [HttpPut("status")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Updating payment status for Payment {PaymentId} to {Status}", 
                    request.PaymentId, request.NewStatus);

                var response = await _paymentService.UpdatePaymentStatusAsync(request);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Payment status updated successfully for Payment {PaymentId}", request.PaymentId);
                    return Ok(response);
                }
                
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for Payment {PaymentId}", request.PaymentId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    BadRequest<string>("An error occurred while updating payment status"));
            }
        }

        /// <summary>
        /// Handle Paymob payment callback (user redirect)
        /// Transaction Response Callback - returns user to your website
        /// </summary>
        /// <returns>HTML response for user</returns>
        [HttpGet("callback")]
        [HttpPost("callback")]
        public async Task<IActionResult> HandleCallback()
        {
            try
            {
                _logger.LogInformation("=== CALLBACK DEBUG START ===");
                _logger.LogInformation("Method: {Method}, URL: {Url}", Request.Method, Request.GetDisplayUrl());
                _logger.LogInformation("Headers: {Headers}", string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}")));
                _logger.LogInformation("Query: {Query}", Request.QueryString);
                
                _logger.LogInformation("Handling Paymob callback with method {Method}, query: {Query}", 
                    Request.Method, Request.QueryString);

                // Create a unified way to access data from both form and query
                Dictionary<string, string> data = new();
                
                if (Request.Method == "POST" && Request.HasFormContentType)
                {
                    // For POST requests with form data
                    _logger.LogInformation("Using POST form data");
                    foreach (var item in Request.Form)
                    {
                        data[item.Key] = item.Value.ToString();
                    }
                }
                else
                {
                    // For GET requests or POST without form data, use query parameters
                    _logger.LogInformation("Using query parameters");
                    foreach (var item in Request.Query)
                    {
                        data[item.Key] = item.Value.ToString();
                    }
                }

                // Log all received data for debugging
                _logger.LogInformation("Received callback data: {Data}", 
                    string.Join(", ", data.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                var callbackRequest = new PaymentCallbackRequest(
                    data.GetValueOrDefault("merchant_order_id", ""), // Use merchant_order_id as TransactionId
                    data.GetValueOrDefault("success", ""),
                    data.GetValueOrDefault("amount_cents", ""),
                    data.GetValueOrDefault("created_at", ""),
                    data.GetValueOrDefault("currency", ""),
                    data.GetValueOrDefault("error_occured", ""),
                    data.GetValueOrDefault("has_parent_transaction", ""),
                    data.GetValueOrDefault("id", ""),
                    data.GetValueOrDefault("integration_id", ""),
                    data.GetValueOrDefault("is_3d_secure", ""),
                    data.GetValueOrDefault("is_auth", ""),
                    data.GetValueOrDefault("is_capture", ""),
                    data.GetValueOrDefault("is_refunded", ""),
                    data.GetValueOrDefault("is_standalone_payment", ""),
                    data.GetValueOrDefault("is_voided", ""),
                    data.GetValueOrDefault("order", ""),
                    data.GetValueOrDefault("owner", ""),
                    data.GetValueOrDefault("pending", ""),
                    data.GetValueOrDefault("source_data_pan", ""),
                    data.GetValueOrDefault("source_data_sub_type", ""),
                    data.GetValueOrDefault("source_data_type", ""),
                    data.GetValueOrDefault("merchant_order_id", ""),
                    data.GetValueOrDefault("hmac", "")
                );

                _logger.LogInformation("=== PAYMENT LOOKUP ===");
                _logger.LogInformation("Looking for payment with TransactionId: {TransactionId}", callbackRequest.MerchantOrderId);
                
                var response = await _paymentService.HandlePaymentCallbackAsync(callbackRequest);
                
                _logger.LogInformation("=== RESPONSE GENERATION ===");
                _logger.LogInformation("Service Response - Success: {Success}, Message: {Message}", response.Succeeded, response.Message);
                _logger.LogInformation("Callback Success Parameter: {CallbackSuccess}", callbackRequest.Success);
                
                string htmlContent;
                // SUCCESS DETERMINATION: Payment is successful ONLY if the Paymob success parameter is "true"
                // The service response being successful just means we processed the callback correctly
                bool isPaymentSuccessful = callbackRequest.Success.ToLower() == "true";
                
                _logger.LogInformation("Final Payment Success Determination: {IsSuccessful} (based on success={SuccessParam})", isPaymentSuccessful, callbackRequest.Success);
                
                if (isPaymentSuccessful)
                {
                    _logger.LogInformation("Payment callback handled successfully - payment succeeded");
                    htmlContent = HtmlGenerator.GenerateSuccessHtml();
                }
                else
                {
                    _logger.LogWarning("Payment callback - payment was unsuccessful. Success Parameter: {PaymentSuccess}, Service Message: {Message}", 
                        callbackRequest.Success, response.Message);
                    htmlContent = HtmlGenerator.GenerateFailureHtml();
                }

                _logger.LogInformation("=== HTML RESPONSE ===");
                _logger.LogInformation("Generated HTML length: {Length}", htmlContent.Length);
                _logger.LogInformation("Payment Result - Service Success: {ServiceSuccess}, Payment Success: {PaymentSuccess}, Final Result: {FinalResult}", 
                    response.Succeeded, callbackRequest.Success, isPaymentSuccessful);

                // Return proper HTML response with explicit headers for ngrok compatibility
                var contentBytes = Encoding.UTF8.GetBytes(htmlContent);
                Response.Headers["Content-Length"] = contentBytes.Length.ToString();
                Response.Headers["Cache-Control"] = "no-cache";
                Response.Headers["X-Content-Type-Options"] = "nosniff";
                
                _logger.LogInformation("=== FINAL RESPONSE ===");
                _logger.LogInformation("Content-Type: text/html; charset=utf-8");
                _logger.LogInformation("Content-Length: {Length}", contentBytes.Length);
                _logger.LogInformation("Status: 200");
                _logger.LogInformation("=== CALLBACK DEBUG END ===");
                
                return new ContentResult
                {
                    Content = htmlContent,
                    ContentType = "text/html; charset=utf-8",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment callback");
                var securityHtml = HtmlGenerator.GenerateSecurityHtml();
                
                // Return proper error HTML response with explicit headers
                var errorBytes = Encoding.UTF8.GetBytes(securityHtml);
                Response.Headers["Content-Length"] = errorBytes.Length.ToString();
                Response.Headers["Cache-Control"] = "no-cache";
                
                _logger.LogError("Returning error HTML response, length: {Length}", errorBytes.Length);
                
                return new ContentResult
                {
                    Content = securityHtml,
                    ContentType = "text/html; charset=utf-8",
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// Handle Paymob webhook (server-to-server notification)
        /// Transaction Processed Callback - Paymob sends this directly to your server
        /// </summary>
        /// <returns>Acknowledgment response</returns>
        [HttpPost("webhook")]
        [HttpGet("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                _logger.LogInformation("=== WEBHOOK DEBUG START ===");
                _logger.LogInformation("Method: {Method}, Headers: {Headers}", 
                    Request.Method, string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}")));

                // Read raw body for HMAC validation with enhanced error handling
                string body = string.Empty;
                try
                {
                    // Check if this is a GET request first (browser requests from callback)
                    if (Request.Method == "GET" && Request.Query.Any())
                    {
                        _logger.LogInformation("Webhook received as GET request - treating as callback");
                        return await HandleWebhookAsCallback();
                    }

                    // Enable buffering FIRST to allow multiple reads
                    Request.EnableBuffering();
                    
                    // Check if there's actually content to read
                    if (Request.ContentLength == 0 || Request.ContentLength == null)
                    {
                        _logger.LogWarning("Content-Length is 0 or null for webhook request");
                        if (Request.Query.Any())
                        {
                            _logger.LogInformation("Falling back to query parameters");
                            return await HandleWebhookAsCallback();
                        }
                        return BadRequest(new { status = "error", message = "No request content available" });
                    }

                    // Use Request.BodyReader for more reliable reading of chunked/buffered content
                    var bodyBytes = new List<byte>();
                    var buffer = new byte[4096];
                    
                    // Reset position if possible
                    if (Request.Body.CanSeek)
                    {
                        Request.Body.Position = 0;
                    }
                    
                    int bytesRead;
                    while ((bytesRead = await Request.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < bytesRead; i++)
                        {
                            bodyBytes.Add(buffer[i]);
                        }
                    }
                    
                    body = Encoding.UTF8.GetString(bodyBytes.ToArray());
                    
                    // Reset position for any subsequent reads if possible
                    if (Request.Body.CanSeek)
                    {
                        Request.Body.Position = 0;
                    }
                    
                    _logger.LogInformation("Webhook body length: {Length}, Content-Length header: {ContentLength}", body?.Length ?? 0, Request.ContentLength);
                    _logger.LogInformation("Webhook body content: {Body}", string.IsNullOrEmpty(body) ? "<EMPTY>" : (body.Length > 500 ? body.Substring(0, 500) + "..." : body));
                }
                catch (Exception bodyEx)
                {
                    _logger.LogError(bodyEx, "Error reading webhook body: {Message}", bodyEx.Message);
                    
                    // If we can't read the body, try to handle it as a GET request or query parameters
                    if (Request.Query.Any())
                    {
                        _logger.LogInformation("Body read failed - treating as GET request with query parameters");
                        return await HandleWebhookAsCallback();
                    }
                    
                    _logger.LogError("Unable to read request body and no query parameters available");
                    return BadRequest(new { status = "error", message = "Unable to read request content" });
                }

                // Check if body is empty or whitespace
                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogWarning("Received empty webhook body - checking if this is a GET webhook");
                    if (Request.Method == "GET" && Request.Query.Any())
                    {
                        _logger.LogInformation("Processing webhook as GET request");
                        return await HandleWebhookAsCallback();
                    }
                    return BadRequest(new { status = "error", message = "Empty request body" });
                }

                // Parse webhook request - data comes in this format:
                // {"type": "TRANSACTION", "obj": {...}}
                PaymentWebhookData? webhookData;
                try
                {
                    webhookData = JsonSerializer.Deserialize<PaymentWebhookData>(body, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse webhook JSON: {Body}", body);
                    return BadRequest(new { status = "error", message = $"Invalid JSON format: {ex.Message}" });
                }

                if (webhookData?.Obj == null)
                {
                    _logger.LogError("Webhook object is null or invalid structure. Body: {Body}", body);
                    return BadRequest(new { status = "error", message = "Invalid webhook structure - missing 'obj' property" });
                }

                // Validate HMAC if configured
                var hmacSecret = _configuration["Paymob:HMAC"];
                var enforceHmac = _configuration.GetValue<bool>("Paymob:EnforceHMAC", false); // Add this config setting
                
                if (!string.IsNullOrEmpty(hmacSecret))
                {
                    var queryHmac = Request.Query["hmac"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(queryHmac))
                    {
                        if (!_paymobService.ValidateWebhookHmac(body, queryHmac, hmacSecret))
                        {
                            _logger.LogWarning("Invalid HMAC for webhook transaction {TransactionId}", webhookData.Obj.Id);
                            
                            if (enforceHmac)
                            {
                                return Unauthorized(new { status = "error", message = "Invalid signature" });
                            }
                            else
                            {
                                _logger.LogWarning("HMAC enforcement disabled - continuing with webhook processing");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("HMAC validation successful");
                        }
                    }
                }

                // Process the webhook
                var result = await _paymentService.ProcessWebhookAsync(webhookData);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("Webhook processed successfully for transaction {TransactionId}", 
                        webhookData.Obj.Id);
                    return Ok(new { status = "success" });
                }
                
                _logger.LogWarning("Webhook processing failed: {Message}", result.Message);
                return BadRequest(new { status = "error", message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error processing webhook");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Handle Paymob server-to-server notification (Legacy - for backward compatibility)
        /// </summary>
        /// <returns>Acknowledgment response</returns>
        [HttpPost("notification")]
        public async Task<IActionResult> HandleNotification()
        {
            try
            {
                // Read the raw body
                var json = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                _logger.LogInformation("Handling Paymob notification: {Request}", json);

                // Try to parse as webhook first
                try
                {
                    var webhookData = JsonSerializer.Deserialize<PaymentWebhookData>(json, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (webhookData?.Obj != null)
                    {
                        // Use the new webhook handler
                        var result = await _paymentService.ProcessWebhookAsync(webhookData);
                        
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Payment notification handled successfully via webhook format");
                            return Ok(new { status = "success" });
                        }
                        
                        _logger.LogWarning("Payment notification failed: {Message}", result.Message);
                        return BadRequest(new { status = "failed", message = result.Message });
                    }
                }
                catch (JsonException)
                {
                    // Fall back to legacy handling
                    _logger.LogInformation("Falling back to legacy notification handling");
                }

                // Legacy notification handling (kept for backward compatibility)
                var notificationDict = new Dictionary<string, string>();
                var notification = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                if (notification != null)
                {
                    foreach (var kvp in notification)
                    {
                        notificationDict[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
                    }
                }
                
                var response = await _paymentService.HandlePaymentNotificationAsync(notificationDict);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Payment notification handled successfully via legacy format");
                    return Ok(new { status = "success" });
                }
                
                _logger.LogWarning("Payment notification failed: {Message}", response.Message);
                return BadRequest(new { status = "failed", message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment notification");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get payment methods available for frontend
        /// </summary>
        /// <returns>List of available payment methods</returns>
        [HttpGet("methods")]
        public IActionResult GetPaymentMethods()
        {
            try
            {
                var methods = new[]
                {
                    new { id = (int)PaymentMethod.CashOnDelivery, name = "Cash on Delivery", code = "COD" },
                    new { id = (int)PaymentMethod.Card, name = "Credit/Debit Card", code = "Card" },
                    new { id = (int)PaymentMethod.Wallet, name = "Mobile Wallet", code = "Wallet" }
                };

                return Ok(Success(methods));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment methods");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    BadRequest<string>("An error occurred while fetching payment methods"));
            }
        }

        /// <summary>
        /// Handle webhook that comes as GET request (fallback method)
        /// </summary>
        private async Task<IActionResult> HandleWebhookAsCallback()
        {
            try
            {
                _logger.LogInformation("Processing webhook as callback-style GET request");
                
                // Extract data from query parameters
                var data = new Dictionary<string, string>();
                foreach (var item in Request.Query)
                {
                    data[item.Key] = item.Value.ToString();
                }

                // Log received data
                _logger.LogInformation("Webhook GET data: {Data}", 
                    string.Join(", ", data.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                if (!data.ContainsKey("merchant_order_id") || !data.ContainsKey("success"))
                {
                    _logger.LogError("Missing required webhook parameters: merchant_order_id or success");
                    return BadRequest(new { status = "error", message = "Missing required parameters" });
                }

                var callbackRequest = new PaymentCallbackRequest(
                    data.GetValueOrDefault("merchant_order_id", ""),
                    data.GetValueOrDefault("success", ""),
                    data.GetValueOrDefault("amount_cents", ""),
                    data.GetValueOrDefault("created_at", ""),
                    data.GetValueOrDefault("currency", ""),
                    data.GetValueOrDefault("error_occured", ""),
                    data.GetValueOrDefault("has_parent_transaction", ""),
                    data.GetValueOrDefault("id", ""),
                    data.GetValueOrDefault("integration_id", ""),
                    data.GetValueOrDefault("is_3d_secure", ""),
                    data.GetValueOrDefault("is_auth", ""),
                    data.GetValueOrDefault("is_capture", ""),
                    data.GetValueOrDefault("is_refunded", ""),
                    data.GetValueOrDefault("is_standalone_payment", ""),
                    data.GetValueOrDefault("is_voided", ""),
                    data.GetValueOrDefault("order", ""),
                    data.GetValueOrDefault("owner", ""),
                    data.GetValueOrDefault("pending", ""),
                    data.GetValueOrDefault("source_data_pan", ""),
                    data.GetValueOrDefault("source_data_sub_type", ""),
                    data.GetValueOrDefault("source_data_type", ""),
                    data.GetValueOrDefault("merchant_order_id", ""),
                    data.GetValueOrDefault("hmac", "")
                );

                var response = await _paymentService.HandlePaymentCallbackAsync(callbackRequest);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Webhook processed successfully via GET method");
                    return Ok(new { status = "success" });
                }
                
                _logger.LogWarning("Webhook processing failed: {Message}", response.Message);
                return BadRequest(new { status = "error", message = response.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook as GET request");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { status = "error", message = "Internal server error" });
            }
        }

    }

}