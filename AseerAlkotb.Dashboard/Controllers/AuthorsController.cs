using System.Globalization;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Mapping;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Dashboard.Helpers;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            // حد أدنى وأقصى عشان pageNumber و pageSize ميبقوش حمير
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, Math.Min(100, pageSize)); // مثلاً مش أكتر من 100 عشان الداتا بيس ميتعبش

            var request = new GetAllAuthorsPaginatedRequest(pageNumber, pageSize, search);
            var result = await _authorServices.GetAllAuthorsPaginatedAsync(request);

            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message ?? "Failed to load authors";


                // Set ViewBag values even for error case to prevent null reference
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalCount = 0;
                ViewBag.SearchTerm = search ?? "";

                return View(new List<GetAllAuthorsPaginatedResponse>());
            }

            // Set all required ViewBag values
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalCount = result.TotalCount; // This was missing!
            ViewBag.SearchTerm = search ?? "";

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
            ViewBag.CountryList = CountryHelper.BuildCountrySelectList();
            return View();
        }


        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAuthorRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Extract English fields from form
                    var nameEn = Request.Form["EnglishName"].ToString();
                    var bioEn = Request.Form["EnglishBio"].ToString();

                    // Create new request with English fields
                    var updatedRequest = request with
                    {
                        Name_en = !string.IsNullOrWhiteSpace(nameEn) ? nameEn : null,
                        Bio_en = !string.IsNullOrWhiteSpace(bioEn) ? bioEn : null
                    };

                    var result = await _authorServices.AddAuthorAsync(updatedRequest);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Author created successfully!";
                        return RedirectToAction(nameof(Index));
                    }

                    // Handle validation errors from FluentValidation
                    if (result.Errors != null && result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            foreach (var error2 in error.Value)
                                ModelState.AddModelError(string.Empty, error2);
                        }
                    }
                    else
                    {
                        TempData["Error"] = result.Message ?? "Failed to create author. Please check your input and try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the author. Please try again.";
                // Log the exception if you have logging configured
            }

            return View(request);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(id));
            if (!response.Succeeded || response.Data == null)
                return NotFound();

            var countryCode = Enum.Parse<CountryCode>(response.Data.CountryCode);

            var request = new UpdateAuthorResponse(
                response.Data.Id,
                response.Data.Name,
                response.Data.Bio,
                response.Data.ImageUrl,
                countryCode
            );

            ViewBag.CountryList = CountryHelper.BuildCountrySelectList(countryCode.ToString());
            return View(request);
        }


        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAuthorRequest request)
        {
            try
            {
                // إضافة الـ Id إلى الـ Request
                request = request with { Id = id };

                if (ModelState.IsValid)
                {
                    // Extract English fields from form
                    var nameEn = Request.Form["EnglishName"].ToString();
                    var bioEn = Request.Form["EnglishBio"].ToString();

                    // Create new request with English fields
                    var updatedRequest = request with
                    {
                        Name_en = !string.IsNullOrWhiteSpace(nameEn) ? nameEn : null,
                        Bio_en = !string.IsNullOrWhiteSpace(bioEn) ? bioEn : null
                    };

                    var result = await _authorServices.UpdateAuthorAsync(updatedRequest);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Author updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }

                    // Handle validation errors from FluentValidation
                    if (result.Errors != null && result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            foreach (var error2 in error.Value)
                                ModelState.AddModelError(string.Empty, error2);
                        }
                    }
                    else
                    {
                        TempData["Error"] = result.Message ?? "Failed to update author. Please check your input and try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the author. Please try again.";
                // Log the exception if you have logging configured
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

       
