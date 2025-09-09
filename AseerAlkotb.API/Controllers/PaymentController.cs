﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using AseerAlkotb.API.Helpers;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("CallbackPolicy")]
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

                // Extract source_data fields properly - Paymob sends them as separate parameters
                var sourceDataPan = data.ContainsKey("source_data.pan") ? data["source_data.pan"] : 
                                   (data.ContainsKey("source_data_pan") ? data["source_data_pan"] : "");
                var sourceDataSubType = data.ContainsKey("source_data.sub_type") ? data["source_data.sub_type"] : 
                                       (data.ContainsKey("source_data_sub_type") ? data["source_data_sub_type"] : "");
                var sourceDataType = data.ContainsKey("source_data.type") ? data["source_data.type"] : 
                                    (data.ContainsKey("source_data_type") ? data["source_data_type"] : "");

                // Enhanced logging for critical fields with validation
                _logger.LogInformation("Critical HMAC Fields:");
                _logger.LogInformation("  amount_cents: '{AmountCents}'", data.GetValueOrDefault("amount_cents", ""));
                _logger.LogInformation("  created_at: '{CreatedAt}'", data.GetValueOrDefault("created_at", ""));
                _logger.LogInformation("  currency: '{Currency}'", data.GetValueOrDefault("currency", ""));
                _logger.LogInformation("  error_occured: '{ErrorOccured}'", data.GetValueOrDefault("error_occured", ""));
                _logger.LogInformation("  has_parent_transaction: '{HasParentTransaction}'", data.GetValueOrDefault("has_parent_transaction", ""));
                _logger.LogInformation("  id: '{Id}'", data.GetValueOrDefault("id", ""));
                _logger.LogInformation("  integration_id: '{IntegrationId}'", data.GetValueOrDefault("integration_id", ""));
                _logger.LogInformation("  is_3d_secure: '{Is3dSecure}'", data.GetValueOrDefault("is_3d_secure", ""));
                _logger.LogInformation("  is_auth: '{IsAuth}'", data.GetValueOrDefault("is_auth", ""));

                // Validate required parameters
                if (!ValidateRequiredParameters(data))
                {
                    _logger.LogError("Missing required callback parameters");
                    string errorHtml = GenerateErrorResponse("Missing required parameters", "callback");
                    return new ContentResult
                    {
                        Content = errorHtml,
                        ContentType = "text/html; charset=utf-8",
                        StatusCode = 400
                    };
                }

                // Sanitize input parameters
                data = SanitizeParameters(data);

                // Validate timestamp to prevent replay attacks
                if (!ValidateTimestamp(data.GetValueOrDefault("created_at", "")))
                {
                    _logger.LogError("Invalid or expired timestamp in callback");
                    string errorHtml = GenerateErrorResponse("Request timestamp is invalid or expired", "callback");
                    return new ContentResult
                    {
                        Content = errorHtml,
                        ContentType = "text/html; charset=utf-8",
                        StatusCode = 400
                    };
                }
                _logger.LogInformation("  is_capture: '{IsCapture}'", data.GetValueOrDefault("is_capture", ""));
                _logger.LogInformation("  is_refunded: '{IsRefunded}'", data.GetValueOrDefault("is_refunded", ""));
                _logger.LogInformation("  is_standalone_payment: '{IsStandalonePayment}'", data.GetValueOrDefault("is_standalone_payment", ""));
                _logger.LogInformation("  is_voided: '{IsVoided}'", data.GetValueOrDefault("is_voided", ""));
                _logger.LogInformation("  order: '{Order}'", data.GetValueOrDefault("order", ""));
                _logger.LogInformation("  owner: '{Owner}'", data.GetValueOrDefault("owner", ""));
                _logger.LogInformation("  pending: '{Pending}'", data.GetValueOrDefault("pending", ""));
                _logger.LogInformation("  source_data.pan: '{SourceDataPan}'", sourceDataPan);
                _logger.LogInformation("  source_data.sub_type: '{SourceDataSubType}'", sourceDataSubType);
                _logger.LogInformation("  source_data.type: '{SourceDataType}'", sourceDataType);
                _logger.LogInformation("  success: '{Success}'", data.GetValueOrDefault("success", ""));
                _logger.LogInformation("  hmac: '{Hmac}'", data.GetValueOrDefault("hmac", ""));

                var callbackRequest = new PaymentCallbackRequest(
                    data.GetValueOrDefault("id", ""), // TransactionId
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
                    sourceDataPan,
                    sourceDataSubType,
                    sourceDataType,
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
                // SECURITY-FIRST SUCCESS DETERMINATION: Payment is successful ONLY if:
                // 1. Service response succeeded (includes HMAC validation)
                // 2. Paymob success parameter is "true"
                bool isServiceSuccessful = response.Succeeded;
                bool isPaymobSuccessful = callbackRequest.Success.ToLower() == "true";
                bool isPaymentSuccessful = isServiceSuccessful && isPaymobSuccessful;

                _logger.LogInformation("Final Payment Success Determination: Service={ServiceSuccess}, Paymob={PaymobSuccess}, Final={FinalSuccess}", 
                    isServiceSuccessful, isPaymobSuccessful, isPaymentSuccessful);

                if (isPaymentSuccessful)
                {
                    _logger.LogInformation("Payment callback handled successfully - payment succeeded");
                    htmlContent = HtmlGenerator.GenerateSuccessHtml();
                }
                else if (!isServiceSuccessful)
                {
                    _logger.LogError("Payment callback - service validation failed (likely HMAC). Success Parameter: {PaymentSuccess}, Service Message: {Message}",
                        callbackRequest.Success, response.Message);
                    htmlContent = HtmlGenerator.GenerateSecurityHtml();
                }
                else
                {
                    _logger.LogWarning("Payment callback - payment was unsuccessful. Success Parameter: {PaymentSuccess}, Service Message: {Message}",
                        callbackRequest.Success, response.Message);
                    htmlContent = HtmlGenerator.GenerateFailureHtml();
                }

                _logger.LogInformation("=== HTML RESPONSE ===");
                _logger.LogInformation("Generated HTML length: {Length}", htmlContent.Length);
                _logger.LogInformation("Payment Result - Service Success: {ServiceSuccess}, Paymob Success: {PaymobSuccess}, Final Result: {FinalResult}",
                    isServiceSuccessful, isPaymobSuccessful, isPaymentSuccessful);

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
        [EnableRateLimiting("WebhookPolicy")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                _logger.LogInformation("=== WEBHOOK DEBUG START ===");
                _logger.LogInformation("Method: {Method}, Headers: {Headers}",
                    Request.Method, string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}")));

                // Validate source IP address
                if (!ValidateSourceIP())
                {
                    _logger.LogWarning("Webhook request from unauthorized IP address: {IP}", GetClientIPAddress());
                    return Unauthorized(new { status = "error", message = "Unauthorized IP address" });
                }

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

                // Validate HMAC if configured - enforce by default in production
                var hmacSecret = _configuration["Paymob:HMAC"];
                var isProduction = _configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production";
                var enforceHmac = _configuration.GetValue<bool>("Paymob:EnforceHMAC", isProduction); // Default TRUE in production

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

        /// <summary>
        /// Validate source IP address against whitelist
        /// </summary>
        private bool ValidateSourceIP()
        {
            try
            {
                var clientIP = GetClientIPAddress();
                var whitelistedIPs = _configuration.GetSection("Paymob:WhitelistedIPs").Get<string[]>();
                
                // If no whitelist configured, allow all (for development)
                if (whitelistedIPs == null || whitelistedIPs.Length == 0)
                {
                    _logger.LogWarning("No IP whitelist configured - allowing all IPs (not recommended for production)");
                    return true;
                }
                
                var isWhitelisted = whitelistedIPs.Contains(clientIP, StringComparer.OrdinalIgnoreCase);
                
                _logger.LogInformation("IP validation - Client: {ClientIP}, Whitelisted: {IsWhitelisted}", 
                    clientIP, isWhitelisted);
                
                return isWhitelisted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating source IP");
                return false; // Fail secure
            }
        }

        /// <summary>
        /// Get client IP address from request
        /// </summary>
        private string GetClientIPAddress()
        {
            try
            {
                // Check for X-Forwarded-For header (common with proxies/load balancers)
                var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    // Take the first IP if multiple are present
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Check for X-Real-IP header (nginx)
                var realIP = Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIP))
                {
                    return realIP.Trim();
                }

                // Fall back to connection remote IP
                return Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client IP address");
                return "Unknown";
            }
        }

        /// <summary>
        /// Validate timestamp to prevent replay attacks (5-minute window)
        /// </summary>
        private bool ValidateTimestamp(string timestamp)
        {
            if (string.IsNullOrEmpty(timestamp))
            {
                _logger.LogWarning("Timestamp is missing from request");
                return true; // Allow if timestamp is missing for backward compatibility
            }

            try
            {
                // Try parsing different timestamp formats
                DateTime callbackTime;
                
                // Try parsing with multiple formats to handle timezone issues
                if (DateTimeOffset.TryParse(timestamp, out var callbackTimeOffset))
                {
                    // Use DateTimeOffset to properly handle timezone information
                    callbackTime = callbackTimeOffset.UtcDateTime;
                }
                else if (DateTime.TryParse(timestamp, out callbackTime))
                {
                    // Fallback to DateTime parsing
                    // For Paymob timestamps, assume they are UTC if unspecified
                    if (callbackTime.Kind == DateTimeKind.Unspecified)
                    {
                        callbackTime = DateTime.SpecifyKind(callbackTime, DateTimeKind.Utc);
                    }
                    else if (callbackTime.Kind == DateTimeKind.Local)
                    {
                        callbackTime = callbackTime.ToUniversalTime();
                    }
                }
                else
                {
                    _logger.LogError("Failed to parse timestamp: {Timestamp}", timestamp);
                    return false;
                }

                var currentTime = DateTime.UtcNow;
                var age = currentTime - callbackTime;
                
                // Get timestamp validation window from configuration (default 5 minutes)
                var validationWindowMinutes = _configuration.GetValue<int>("Paymob:TimestampValidationWindowMinutes", 5);
                var isValid = Math.Abs(age.TotalMinutes) <= validationWindowMinutes;
                
                _logger.LogInformation("Timestamp validation - Current: {CurrentTime}, Callback: {CallbackTime}, Age: {Age} minutes, Window: {Window} minutes", 
                    currentTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), callbackTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), age.TotalMinutes, validationWindowMinutes);
                
                if (!isValid)
                {
                    _logger.LogWarning("Timestamp outside valid window: {Timestamp}, Age: {Age} minutes", 
                        timestamp, age.TotalMinutes);
                }
                else
                {
                    _logger.LogInformation("Timestamp validation passed for: {Timestamp}", timestamp);
                }
                
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating timestamp: {Timestamp}", timestamp);
                return false;
            }
        }

        /// <summary>
        /// Validate required parameters according to security specifications
        /// </summary>
        private bool ValidateRequiredParameters(Dictionary<string, string> data)
        {
            var requiredParams = new[] { "merchant_order_id", "success" };
            
            foreach (var param in requiredParams)
            {
                if (!data.ContainsKey(param) || string.IsNullOrWhiteSpace(data[param]))
                {
                    _logger.LogError("Missing required parameter: {Parameter}", param);
                    return false;
                }
            }

            // Validate parameter formats
            if (data.ContainsKey("amount_cents") && !string.IsNullOrEmpty(data["amount_cents"]))
            {
                if (!decimal.TryParse(data["amount_cents"], out _))
                {
                    _logger.LogError("Invalid amount_cents format: {Value}", data["amount_cents"]);
                    return false;
                }
            }

            if (!bool.TryParse(data["success"], out _))
            {
                _logger.LogError("Invalid success format: {Value}", data["success"]);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sanitize input parameters to prevent injection attacks
        /// </summary>
        private Dictionary<string, string> SanitizeParameters(Dictionary<string, string> data)
        {
            var sanitized = new Dictionary<string, string>();
            
            foreach (var kvp in data)
            {
                var key = kvp.Key?.Trim();
                var value = kvp.Value?.Trim();
                
                // Remove potential malicious content
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    // Basic sanitization - remove script tags and SQL injection attempts
                    value = value.Replace("<script", "", StringComparison.OrdinalIgnoreCase)
                                .Replace("</script>", "", StringComparison.OrdinalIgnoreCase)
                                .Replace("javascript:", "", StringComparison.OrdinalIgnoreCase)
                                .Replace("'; DROP TABLE", "", StringComparison.OrdinalIgnoreCase)
                                .Replace("'; DELETE FROM", "", StringComparison.OrdinalIgnoreCase);
                    
                    sanitized[key] = value;
                }
            }
            
            return sanitized;
        }

        /// <summary>
        /// Generate error response HTML
        /// </summary>
        private string GenerateErrorResponse(string error, string type)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Payment Error</title>
                <meta charset='utf-8'>
            </head>
            <body>
                <h1>Payment Processing Error</h1>
                <p>Error: {error}</p>
                <p>Type: {type}</p>
                <p>Please contact support if this issue persists.</p>
            </body>
            </html>";
        }

    }

}