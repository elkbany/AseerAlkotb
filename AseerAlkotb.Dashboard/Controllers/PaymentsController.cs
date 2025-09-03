using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Payments.Requests;
using AseerAlkotb.Application.Features.Payments.Responses;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
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
            // Always set enum values for dropdowns first (before any potential errors)
            ViewBag.PaymentStatuses = Enum.GetValues<PaymentStatus>();
            ViewBag.PaymentMethods = Enum.GetValues<PaymentMethod>();
            
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

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);

            if (!result.Succeeded || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Payment not found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PaymentStatuses = Enum.GetValues<PaymentStatus>();
            return View(result.Data);
        }

        // POST: Payments/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request data" });
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
                PaymentMethod.MobileWallet => "fas fa-mobile-alt",
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

        // POST: Payments/BulkUpdateStatus (Future implementation)
        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] List<int> paymentIds, PaymentStatus newStatus)
        {
            try
            {
                var successCount = 0;
                var failCount = 0;

                foreach (var paymentId in paymentIds)
                {
                    var request = new UpdatePaymentStatusRequest(paymentId, newStatus);
                    var result = await _paymentService.UpdatePaymentStatusAsync(request);
                    
                    if (result.Succeeded)
                        successCount++;
                    else
                        failCount++;
                }

                return Json(new
                {
                    success = true,
                    message = $"Updated {successCount} payments successfully. {failCount} failed.",
                    successCount,
                    failCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update payments" });
            }
        }
    }
}