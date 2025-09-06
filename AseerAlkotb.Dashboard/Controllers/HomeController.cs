using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Dashboard.Models;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Add this namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this namespace
using System.Diagnostics;
using System.Threading.Tasks;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookServices _bookServices;
        private readonly IAuthorServices _authorServices;
        private readonly IOrderServices _orderServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IPublisherServices _publisherServices;
        // Inject UserManager directly
        private readonly UserManager<User> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            IBookServices bookServices,
            IAuthorServices authorServices,
            IOrderServices orderServices,
            ICategoryServices categoryServices,
            IPublisherServices publisherServices,
            UserManager<User> userManager) // Add the new dependency
        {
            _logger = logger;
            _bookServices = bookServices;
            _authorServices = authorServices;
            _orderServices = orderServices;
            _categoryServices = categoryServices;
            _publisherServices = publisherServices;
            _userManager = userManager; // Initialize it
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                //Fetch total counts for all entities.

                //Publishers
   
               var publishersRequest = new GetAllPublishersPaginatedRequest();
                var publishersResult = await _publisherServices.GetAllPublishersPaginatedAsync(publishersRequest);
                ViewBag.TotalPublishers = publishersResult.TotalCount;

                // Books
                var booksRequest = new GetAllBooksPaginatedRequest();
                var booksResult = await _bookServices.GetAllBooksPaginatedAsync(booksRequest);
                ViewBag.TotalBooks = booksResult.TotalCount;

                // Authors
                var authorsRequest = new GetAllAuthorsPaginatedRequest();
                var authorsResult = await _authorServices.GetAllAuthorsPaginatedAsync(authorsRequest);
                ViewBag.TotalAuthors = authorsResult.TotalCount;

                // Categories
                var categoriesRequest = new GetAllCategoriesPaginatedRequest();
                var categoriesResult = await _categoryServices.GetAllCategoriesPaginatedAsync(categoriesRequest);
                ViewBag.TotalCategories = categoriesResult.TotalCount;

                // Orders
                var ordersRequest = new GetAllOrdersPaginatedRequest(null, null);
                var ordersResult = await _orderServices.GetAllOrdersPaginatedByAdminAsync(ordersRequest);
                ViewBag.TotalOrders = ordersResult.TotalCount;

                // Users - Get the total count directly from UserManager
                ViewBag.TotalUsers = await _userManager.Users.CountAsync();

                return View();
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dashboard data.");

                // Fallback to default values
                ViewBag.TotalBooks = 0;
                ViewBag.TotalAuthors = 0;
                ViewBag.TotalOrders = 0;
                ViewBag.TotalCategories = 0;
                ViewBag.TotalPublishers = 0;
                ViewBag.TotalUsers = 0;

                return View();
    }
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