using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Mvc;
using AseerAlkotb.Application.Contracts;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class PublishersController : Controller
    {
        private readonly IPublisherServices _publisherService;
        private readonly IConfiguration _configuration;

        public PublishersController(IPublisherServices publisherService, IConfiguration configuration)
        {
            _publisherService = publisherService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var request = new GetAllPublishersPaginatedRequest(pageNumber, pageSize, search);
            var result = await _publisherService.GetAllPublishersPaginatedAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Failed to load publishers";
                return View(new List<GetAllPublisherPaginatedResponse>());
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchTerm = search;
            return View(result.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = new GetPublisherByIdRequest(id);
            var result = await _publisherService.GetPublisherByIdAsync(request);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Publisher not found";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPublisherRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _publisherService.AddPublisherAsync(request);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Publisher created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message;
            }
            return View(request);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _publisherService.GetPublisherByIdAsync(new GetPublisherByIdRequest(id));

            if (!response.Succeeded || response.Data == null)
            {
                return NotFound();
            }

            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var request = new UpdatePublisherRequest(
                response.Data.Id,
                response.Data.Name,
                response.Data.Description,
                null,
                response.Data.ContactEmail
            );

            ViewBag.LogoUrl = $"{apiBaseUrl}{response.Data.LogoUrl}";

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdatePublisherRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (ModelState.IsValid)
            {
                var result = await _publisherService.UpdatePublisherAsync(request);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Publisher updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message;
            }
            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(new GetPublisherByIdRequest(id));
            if (!publisher.Succeeded || publisher.Data == null)
            {
                return NotFound();
            }

            return View(publisher.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _publisherService.DeletePublisherAsync(new DeletePublisherRequest(id));
            if (result.Succeeded)
            {
                TempData["Success"] = "Publisher deleted successfully.";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}