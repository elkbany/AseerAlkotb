using AseerAlkotb.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Add dashboard statistics
            ViewBag.TotalBooks = 0;    // Replace with actual count
            ViewBag.TotalAuthors = 0;  // Replace with actual count
            ViewBag.TotalOrders = 0;   // Replace with actual count
            ViewBag.TotalReviews = 0;  // Replace with actual count

            // Recent Orders - Replace with actual data
            ViewBag.RecentOrders = new List<object>();

            // Recent Books - Replace with actual data
            ViewBag.RecentBooks = new List<object>();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
