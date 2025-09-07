using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: Payments
        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string search = "",
            PaymentStatus? paymentStatus = null,
            PaymentMethod? paymentMethod = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool dateAscending = false)
        {
            try
            {
                // Always set enum values for dropdowns first (before any potential errors)
                ViewBag.PaymentStatuses = Enum.GetValues<PaymentStatus>();
                ViewBag.PaymentMethods = Enum.GetValues<PaymentMethod>();
                
                // Validate pagination parameters
                pageNumber = Math.Max(1, pageNumber);
                pageSize = Math.Max(1, Math.Min(100, pageSize)); // Limit to 100 items per page
                search = search?.Trim() ?? "";
                
                // Validate date range
                if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                {
                    TempData["Warning"] = "From date cannot be later than To date";
                    fromDate = null;
                    toDate = null;
                }
                
                var request = new GetAllPaymentsPaginatedRequest(
                    paymentStatus,
                    paymentMethod,
                    fromDate,
                    toDate,
                    null, // Customer search
                    dateAscending,
                    pageNumber,
                    pageSize,
                    search);

                var result = await _paymentService.GetAllPaymentsPaginatedAsync(request);

                // Pass filter values to view
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchTerm = search;
                ViewBag.SelectedPaymentStatus = paymentStatus;
                ViewBag.SelectedPaymentMethod = paymentMethod;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                ViewBag.DateAscending = dateAscending;

                if (!result.Succeeded || result.Data == null)
                {
                    TempData["Error"] = result.Message ?? "Failed to load payments";
                    ViewBag.TotalPages = 0;
                    ViewBag.TotalCount = 0;
                    return View(new List<GetAllPaymentsPaginatedResponse>());
                }

                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                ViewBag.TotalCount = result.TotalCount;

                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred while loading payments";
                
                // Ensure ViewBag values are set even in error cases
                ViewBag.PaymentStatuses = Enum.GetValues<PaymentStatus>();
                ViewBag.PaymentMethods = Enum.GetValues<PaymentMethod>();
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 0;
                ViewBag.TotalCount = 0;
                ViewBag.SearchTerm = search;
                ViewBag.SelectedPaymentStatus = paymentStatus;
                ViewBag.SelectedPaymentMethod = paymentMethod;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                ViewBag.DateAscending = dateAscending;
                
                return View(new List<GetAllPaymentsPaginatedResponse>());
            }
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid payment ID";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _paymentService.GetPaymentByIdAsync(id);

                if (!result.Succeeded || result.Data == null)
                {
                    TempData["Error"] = result.Message ?? "Payment not found";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.PaymentStatuses = Enum.GetValues<PaymentStatus>();
                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred while loading payment details";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Payments/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                // Validate anti-forgery token for AJAX requests
                var token = Request.Headers["RequestVerificationToken"].FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Invalid security token" });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (request.PaymentId <= 0)
                {
                    return Json(new { success = false, message = "Invalid payment ID" });
                }

                var result = await _paymentService.UpdatePaymentStatusAsync(request);

                if (result.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Payment status updated successfully",
                        newStatus = request.NewStatus.ToString(),
                        updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                    });
                }

                return Json(new { success = false, message = result.Message ?? "Failed to update payment status" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred while updating the payment status" });
            }
        }

        // GET: Payments/GetStatusBadgeClass
        [HttpGet]
        public IActionResult GetStatusBadgeClass(PaymentStatus status)
        {
            var badgeClass = status switch
            {
                PaymentStatus.Pending => "bg-warning",
                PaymentStatus.Processing => "bg-info",
                PaymentStatus.Paid => "bg-success",
                PaymentStatus.Failed => "bg-danger",
                PaymentStatus.Cancelled => "bg-secondary",
                PaymentStatus.Refunded => "bg-dark",
                PaymentStatus.PartiallyRefunded => "bg-primary",
                _ => "bg-secondary"
            };

            return Json(new { badgeClass });
        }

        // GET: Payments/GetMethodIcon
        [HttpGet]
        public IActionResult GetMethodIcon(PaymentMethod method)
        {
            var iconClass = method switch
            {
                PaymentMethod.CashOnDelivery => "fas fa-money-bill-wave",
                PaymentMethod.Card => "fas fa-credit-card",
                PaymentMethod.Wallet => "fas fa-mobile-alt",
                _ => "fas fa-question-circle"
            };

            return Json(new { iconClass });
        }

        // GET: Payments/GetPaymentsByOrder/5
        public async Task<IActionResult> GetPaymentsByOrder(int orderId)
        {
            var result = await _paymentService.GetPaymentsByOrderIdAsync(orderId);

            if (result.Succeeded)
            {
                return Json(new { success = true, data = result.Data });
            }

            return Json(new { success = false, message = result.Message ?? "Failed to retrieve payments" });
        }

        // GET: Payments/Export (Future implementation)
        public async Task<IActionResult> Export()
        {
            TempData["Info"] = "Export functionality will be implemented soon";
            return RedirectToAction(nameof(Index));
        }

        // GET: Payments/Print/5 (Future implementation)
        public async Task<IActionResult> Print(int id)
        {
            TempData["Info"] = "Print functionality will be implemented soon";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Payments/BulkUpdateStatus
        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateRequest request)
        {
            try
            {
                // Validate anti-forgery token for AJAX requests
                var token = Request.Headers["RequestVerificationToken"].FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Invalid security token" });
                }

                if (request.PaymentIds == null || !request.PaymentIds.Any())
                {
                    return Json(new { success = false, message = "No payments selected" });
                }

                if (request.PaymentIds.Count > 50)
                {
                    return Json(new { success = false, message = "Cannot update more than 50 payments at once" });
                }

                var successCount = 0;
                var failCount = 0;
                var errorMessages = new List<string>();

                foreach (var paymentId in request.PaymentIds)
                {
                    var updateRequest = new UpdatePaymentStatusRequest(paymentId, request.NewStatus);
                    var result = await _paymentService.UpdatePaymentStatusAsync(updateRequest);
                    
                    if (result.Succeeded)
                        successCount++;
                    else
                    {
                        failCount++;
                        if (!string.IsNullOrEmpty(result.Message))
                            errorMessages.Add($"Payment {paymentId}: {result.Message}");
                    }
                }

                var message = $"Updated {successCount} payments successfully.";
                if (failCount > 0)
                {
                    message += $" {failCount} failed.";
                }

                return Json(new
                {
                    success = successCount > 0,
                    message,
                    successCount,
                    failCount,
                    errors = errorMessages
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred during bulk update" });
            }
        }
    }

    // Helper class for bulk update request
    public class BulkUpdateRequest
    {
        public List<int> PaymentIds { get; set; } = new();
        public PaymentStatus NewStatus { get; set; }
    }
}