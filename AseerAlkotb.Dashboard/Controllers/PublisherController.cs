using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.ResponseHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
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
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
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
                    try
                    {
                        var id = result.Data.Id;
                        var nameAr = Request.Form["Name"].ToString();
                        var nameEn = Request.Form["EnglishName"].ToString();
                        if (string.IsNullOrWhiteSpace(nameEn)) nameEn = nameAr;
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Name", nameAr, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Name", nameEn, "en");

                        var descAr = Request.Form["Description"].ToString();
                        var descEn = Request.Form["EnglishDescription"].ToString();
                        if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
                        {
                            if (string.IsNullOrWhiteSpace(descEn)) descEn = descAr;
                            if (string.IsNullOrWhiteSpace(descAr)) descAr = descEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Description", descAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Description", descEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
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


            var request = new UpdatePublisherResponse(
                response.Data.Id,
                response.Data.Name,
                response.Data.Description,
                response.Data.LogoUrl,
                response.Data.ContactEmail
            );


            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePublisherRequest request)
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
                    try
                    {
                        var nameAr = Request.Form["Name"].ToString();
                        var nameEn = Request.Form["EnglishName"].ToString();
                        if (string.IsNullOrWhiteSpace(nameEn)) nameEn = nameAr;
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Name", nameAr, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Name", nameEn, "en");

                        var descAr = Request.Form["Description"].ToString();
                        var descEn = Request.Form["EnglishDescription"].ToString();
                        if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
                        {
                            if (string.IsNullOrWhiteSpace(descEn)) descEn = descAr;
                            if (string.IsNullOrWhiteSpace(descAr)) descAr = descEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Description", descAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Publisher_{id}_Description", descEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
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