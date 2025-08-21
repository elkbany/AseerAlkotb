using AseerAlkotb.API.Helpers;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;
using System.Text;

namespace AseerAlkotb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymobService _paymobService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymobService paymobService, IConfiguration configuration)
        {
            _paymobService = paymobService;
            _configuration = configuration;
        }

        [HttpPost("create-payment-token")]
        public async Task<IActionResult> CreatePaymentToken([FromQuery] int orderId, [FromQuery] string paymentMethod)
        {
            if(orderId <= 0 || string.IsNullOrEmpty(paymentMethod))
            {
                return BadRequest("Invalid order ID or payment method.");
            }

            //var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if(string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized("User not Authenticated");
            //}

            try
            {
                if(string.IsNullOrWhiteSpace(paymentMethod))
                {
                    return BadRequest("Payment method is required.");
                }

                if(paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) || 
                   paymentMethod.Equals("wallet", StringComparison.OrdinalIgnoreCase))
                {
                    var request = new ProcessPaymentRequest
                    {
                        OrderId = orderId,
                        PaymentMethod = paymentMethod
                    };
                    var response = await _paymobService.ProcessPaymentAsync(request);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Unsupported payment method. Only 'card' and 'wallet' are supported.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error Processing Payment: {ex.Message}");
            }
            
        }

        [HttpGet("callback")]
        public async Task<IActionResult> CallbackAsync()
        {
            var query = Request.Query;

            string[] fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type" , "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                if (query.TryGetValue(field, out var value))
                {
                    concatenated.Append(value);
                }
                else
                {
                    return BadRequest($"Missing expected field: {field}");
                }
            }

            string recievedHmac = query["hmac"];
            string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), _configuration["Paymob:HMAC"]);

            if (recievedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
            {
                bool success = bool.TryParse(query["success"], out var isSuccess);

                string specialReference = query["merchant_order_id"];
                if (isSuccess)
                {
                    await _paymobService.UpdateOrderSuccess(specialReference);
                    return Content(HtmlGenerator.GenerateSuccessHtml(), "text/html");
                }
                await _paymobService.UpdateOrderFailed(specialReference);
                return Content(HtmlGenerator.GenerateFailureHtml(), "text/html");

            }
            return Content(HtmlGenerator.GenerateSecurityHtml(), "text/html");
        }

    }

}