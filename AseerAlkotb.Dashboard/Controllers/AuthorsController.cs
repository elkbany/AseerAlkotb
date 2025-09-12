using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Mapping;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorServices _authorServices;

        public AuthorsController(IAuthorServices authorServices)
        {
            _authorServices = authorServices;
        }

        // GET: Authors
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var request = new GetAllAuthorsPaginatedRequest(pageNumber, pageSize, search);
            var result = await _authorServices.GetAllAuthorsPaginatedAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Failed to load authors";
                return View(new List<GetAllAuthorsPaginatedResponse>());
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchTerm = search;
            return View(result.Data);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var request = new GetAuthorByIdRequest(id);
            var result = await _authorServices.GetAuthorByIdAsync(request);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Author not found";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAuthorRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authorServices.AddAuthorAsync(request);
                if (result.Succeeded)
                {
                    try
                    {
                        var id = result.Data.Id;
                        var nameAr = Request.Form["Name"].ToString();
                        var nameEn = Request.Form["EnglishName"].ToString();
                        if (string.IsNullOrWhiteSpace(nameEn)) nameEn = nameAr;
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Author_{id}_Name", nameAr, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Author_{id}_Name", nameEn, "en");

                        var bioAr = Request.Form["Bio"].ToString();
                        var bioEn = Request.Form["EnglishBio"].ToString();
                        if (!string.IsNullOrWhiteSpace(bioAr) || !string.IsNullOrWhiteSpace(bioEn))
                        {
                            if (string.IsNullOrWhiteSpace(bioEn)) bioEn = bioAr;
                            if (string.IsNullOrWhiteSpace(bioAr)) bioAr = bioEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Author_{id}_Bio", bioAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Author_{id}_Bio", bioEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message ?? "Failed to create author";
            }
            return View(request);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(id));

            if (!response.Succeeded || response.Data == null)
                return NotFound();

            // استخدام Mapster للتعيين من GetAuthorByIdResponse إلى UpdateAuthorRequest
            // تحويل CountryCode من string إلى enum
            var countryCode = Enum.Parse<CountryCode>(response.Data.CountryCode);
            
            var request = new UpdateAuthorResponse(
                response.Data.Id,
                response.Data.Name,
                response.Data.Bio,
                response.Data.ImageUrl,
                countryCode
            );

            return View(request);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAuthorRequest request)
        {
            // إضافة الـ Id إلى الـ Request
            request = request with { Id = id };
            
            if (ModelState.IsValid)
            {
                await _authorServices.UpdateAuthorAsync(request);
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(id));
            if (!author.Succeeded || author.Data == null)
                return NotFound();
                
            return View(author.Data);
        }

        // POST: Authors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _authorServices.DeleteAuthorAsync(new DeleteAuthorRequest(id));
            return RedirectToAction(nameof(Index));
        }
        
        // ==================== AJAX ENDPOINTS ====================
        
        [HttpGet]
        public async Task<IActionResult> SearchAuthors(string term)
        {
            var authors = await _authorServices.GetAllAuthorsPaginatedAsync(
                new GetAllAuthorsPaginatedRequest { Search = term, PageSize = 10 });

            return Json(authors.Data.Select(a => new { id = a.Id, text = a.Name }));
        }
    }
}
