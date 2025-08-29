using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderServices _orderServices;

        public OrdersController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        // GET: Orders
        public async Task<IActionResult> Index(
            int pageNumber = 1, 
            int pageSize = 10, 
            string search = "", 
            OrderStatus? orderStatus = null,
            EgyptGovernorates? governorate = null,
            bool dateAscending = false)
        {
            var request = new GetAllOrdersPaginatedRequest(
                orderStatus, 
                governorate, 
                dateAscending, 
                pageNumber, 
                pageSize, 
                search);

            var result = await _orderServices.GetAllOrdersPaginatedByAdminAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Failed to load orders";
                return View(new List<object>());
            }

            // Pass filter values to view
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.SearchTerm = search;
            ViewBag.SelectedOrderStatus = orderStatus;
            ViewBag.SelectedGovernorate = governorate;
            ViewBag.DateAscending = dateAscending;
            
            // Pass enum values for dropdowns
            ViewBag.OrderStatuses = Enum.GetValues<OrderStatus>();
            ViewBag.Governorates = Enum.GetValues<EgyptGovernorates>();

            return View(result.Data);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // First get the order by ID to get tracking number
            var orders = await _orderServices.GetAllOrdersPaginatedByAdminAsync(
                new GetAllOrdersPaginatedRequest(null, null, true, 1, 1000, ""));
            
            var order = orders.Data?.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                TempData["Error"] = "Order not found";
                return RedirectToAction(nameof(Index));
            }

            // Get detailed order information using tracking number
            var request = new GetOrderByAdminByTrackingNumberRequest(order.TrackingNumber);
            var result = await _orderServices.GetOrderByTrackingNumberByAdminAsync(request);

            if (!result.Succeeded || result.Data == null)
            {
                TempData["Error"] = result.Message ?? "Order not found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.OrderStatuses = Enum.GetValues<OrderStatus>();
            return View(result.Data);
        }

        // POST: Orders/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            var result = await _orderServices.UpdateOrderStatusAsync(request);

            if (result.Succeeded)
            {
                return Json(new { 
                    success = true, 
                    message = "Order status updated successfully",
                    newStatus = result.Data.Status.ToString(),
                    updatedAt = result.Data.UpdatedAt.ToString("yyyy-MM-dd HH:mm")
                });
            }

            return Json(new { success = false, message = result.Message ?? "Failed to update order status" });
        }

        // GET: Orders/GetStatusBadgeClass
        [HttpGet]
        public IActionResult GetStatusBadgeClass(OrderStatus status)
        {
            var badgeClass = status switch
            {
                OrderStatus.Pending => "bg-warning",
                OrderStatus.Approved => "bg-info", 
                OrderStatus.Shipped => "bg-primary",
                OrderStatus.Delivered => "bg-success",
                OrderStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };

            return Json(new { badgeClass });
        }

        // GET: Orders/Export (Optional - for future implementation)
        public async Task<IActionResult> Export()
        {
            // This can be implemented later for exporting orders to Excel/CSV
            TempData["Info"] = "Export functionality will be implemented soon";
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Print/5 (Optional - for future implementation)
        public async Task<IActionResult> Print(int id)
        {
            // This can be implemented later for printing order details
            TempData["Info"] = "Print functionality will be implemented soon";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}