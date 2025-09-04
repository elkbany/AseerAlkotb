﻿using AseerAlkotb.API.Helpers;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;
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
                    _logger.LogInformation("Payment initialized successfully for Order {OrderId}", request.OrderId);
                    return Ok(response);
                }
                
                _logger.LogWarning("Payment initialization failed for Order {OrderId}: {Message}", 
                    request.OrderId, response.Message);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing payment for Order {OrderId}", request.OrderId);
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
        /// </summary>
        /// <returns>HTML response for user</returns>
        [HttpGet("callback")]
        [HttpPost("callback")]
        public async Task<IActionResult> HandleCallback()
        {
            try
            {
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

                var response = await _paymentService.HandlePaymentCallbackAsync(callbackRequest);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Payment callback handled successfully");
                    return Content(HtmlGenerator.GenerateSuccessHtml(), "text/html");
                }
                
                _logger.LogWarning("Payment callback failed: {Message}", response.Message);
                return Content(HtmlGenerator.GenerateFailureHtml(), "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment callback");
                return Content(HtmlGenerator.GenerateSecurityHtml(), "text/html");
            }
        }

        /// <summary>
        /// Handle Paymob server-to-server notification
        /// </summary>
        /// <param name="request">Notification data from Paymob</param>
        /// <returns>Acknowledgment response</returns>
        [HttpPost("notification")]
        public async Task<IActionResult> HandleNotification([FromBody] object request)
        {
            try
            {
                _logger.LogInformation("Handling Paymob notification: {Request}", request);

                var notificationDict = new Dictionary<string, string>();
                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var notification = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                foreach (var kvp in notification)
                {
                    notificationDict[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
                }
                
                var response = await _paymentService.HandlePaymentNotificationAsync(notificationDict);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Payment notification handled successfully");
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

    }

}