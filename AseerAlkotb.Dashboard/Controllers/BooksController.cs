using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AseerAlkotb.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly IBookServices _bookServices;
        private readonly IAuthorServices _authorServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IPublisherServices _publisherServices;
        private readonly IReviewServices _reviewServices;



        public BooksController(
            IBookServices bookServices,
            IAuthorServices authorServices,
            ICategoryServices categoryServices,
            IPublisherServices publisherServices,
            IReviewServices reviewServices)

        {
            _bookServices = bookServices;
            _authorServices = authorServices;
            _categoryServices = categoryServices;
            _publisherServices = publisherServices;
            _reviewServices = reviewServices;

        }

        public async Task<IActionResult> Index(
    int pageNumber = 1,
    int pageSize = 10,
    string? search = "",
    BookLanguage? language = null,
    List<int>? categoryIds = null,
    List<int>? publisherIds = null,
    BookSortOption? sortBy = null)
        {
            // نجيب ليستات للفلاتر (مرة واحدة لكل عرض)
            var cats = await _categoryServices.GetAllCategoriesPaginatedAsync(
                new GetAllCategoriesPaginatedRequest { PageSize = 100 });
            var pubs = await _publisherServices.GetAllPublishersPaginatedAsync(
                new GetAllPublishersPaginatedRequest { PageSize = 100 });

            ViewBag.Categories = cats.Data.Select(c => new { c.Id, c.Name }).ToList();
            ViewBag.Publishers = pubs.Data.Select(p => new { p.Id, p.Name }).ToList();

            // نخزّن نفس قيم الفلاتر عشان نرجّعها في الواجهة
            ViewBag.SearchTerm = search;
            ViewBag.Language = language;
            ViewBag.CategoryIds = categoryIds ?? new List<int>();
            ViewBag.PublisherIds = publisherIds ?? new List<int>();
            ViewBag.SortBy = sortBy;

            // لو فيه أي فلتر غير البحث أو فيه ترتيب معيّن -> استخدم FilterBooksAsync
            bool hasAdvancedFilters =
                (language != null) ||
                (categoryIds != null && categoryIds.Any()) ||
                (publisherIds != null && publisherIds.Any()) ||
                (sortBy != null);

            if (hasAdvancedFilters || !string.IsNullOrWhiteSpace(search))
            {
                var req = new FilterBooksRequest(
                    SearchTerm: search ?? "",
                    CategoryIds: categoryIds,
                    PublisherIds: publisherIds,
                    Language: language,
                    SortBy: sortBy,
                    PageNumber: pageNumber,
                    PageSize: pageSize
                );

                var result = await _bookServices.FilterBooksAsync(req);

                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                ViewBag.TotalCount = result.TotalCount;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                return View(result);
            }

            // الحالة الافتراضية (من غير فلاتر) – نفس القديم
            var request = new GetAllBooksPaginatedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search ?? ""
            };
            var res = await _bookServices.GetAllBooksPaginatedAsync(request);
            ViewBag.TotalPages = (int)Math.Ceiling((double)res.TotalCount / pageSize);
            ViewBag.TotalCount = res.TotalCount;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            return View(res);
        }


        public async Task<IActionResult> Details(int id)
        {
            var result = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(id));

            if (result == null || result.Data == null)
                return NotFound("Book not found");

            if (result.Data.PublisherId == 0)
                return BadRequest("Book has no publisher assigned");

            // Get book reviews
            var reviewsRequest = new GetAllReviewsPaginatedRequest(
                AuthorId: null,
                BookId: id,
                PageNumber: 1,
                PageSize: 100, // Get all reviews for rating calculation
                Search: ""
            );

            var reviewsResult = await _reviewServices.GetAllReviewsAsync(reviewsRequest);
            var reviews = reviewsResult?.Data ?? new List<AseerAlkotb.Application.Features.Reviews.Responses.GetAllReviewsPaginatedResponse>();

            // Update the book's rating based on reviews
            if (reviews.Any())
            {
                var averageRating = reviews.Average(r => (decimal)r.Rating);
                // You might want to update the book's Rating property here
                // or handle it in your business logic layer
                result.Data.Rating = averageRating;
            }

            var author = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(result.Data.AuthorId));
            var publisher = await _publisherServices.GetPublisherByIdAsync(new GetPublisherByIdRequest(result.Data.PublisherId));

            if (publisher == null || publisher.Data == null)
                return NotFound("Publisher not found");

            var selectedCategories = new List<string>();
            if (result.Data.CategoryIds != null)
            {
                foreach (var catId in result.Data.CategoryIds)
                {
                    var cat = await _categoryServices.GetCategoryByIdAsync(new GetCategoryByIdRequest(catId));
                    if (cat.Succeeded && cat.Data != null)
                        selectedCategories.Add(cat.Data.Name);
                }
            }

            ViewBag.Author = author.Data;
            ViewBag.Publisher = publisher.Data;
            ViewBag.Categories = selectedCategories;
            ViewBag.Reviews = reviews; // Pass reviews to the view
      
            return View(result.Data);
        }

        // Rest of your existing methods remain the same...
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddBookRequest request)
        {
            if (ModelState.IsValid)
            {
                if (request.CategoryIds == null)
                {
                    request = request with { CategoryIds = new List<int>() };
                }

                var result = await _bookServices.AddBookAsync(request);
                if (result.Succeeded)
                {
                    try
                    {
                        var id = result.Data.Id;
                        var titleAr = Request.Form["Title"].ToString();
                        var titleEn = Request.Form["EnglishTitle"].ToString();
                        if (string.IsNullOrWhiteSpace(titleEn)) titleEn = titleAr;
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Title", titleAr, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Title", titleEn, "en");

                        var descAr = Request.Form["Description"].ToString();
                        var descEn = Request.Form["EnglishDescription"].ToString();
                        if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
                        {
                            if (string.IsNullOrWhiteSpace(descEn)) descEn = descAr;
                            if (string.IsNullOrWhiteSpace(descAr)) descAr = descEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Description", descAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Description", descEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message ?? "Failed to create book";
            }

            return View(request);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(id));

            if (!response.Succeeded || response.Data == null)
                return NotFound();


            var model = new UpdateBookResponse(
                response.Data.Id,
                response.Data.Title,
                response.Data.ISBN,
                response.Data.Price,
                response.Data.Description,
                response.Data.DiscountPercentage,
                response.Data.PublishedDate,
                response.Data.PageCount,
                response.Data.Language,
                response.Data.CoverImageUrl,
                response.Data.Format,
                response.Data.StockQuantity,
                response.Data.AuthorId,
                response.Data.PublisherId,
                response.Data.CategoryIds,
                response.Data.IsActive
            );

            var author = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(response.Data.AuthorId));
            var publisher = await _publisherServices.GetPublisherByIdAsync(new GetPublisherByIdRequest(response.Data.PublisherId));

            ViewBag.AuthorName = author?.Data?.Name;
            ViewBag.PublisherName = publisher?.Data?.Name; 

            ViewBag.CategoryIds = response.Data.CategoryIds;
            ViewBag.CategoryNames = response.Data.CategoryNames;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,UpdateBookRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _bookServices.UpdateBookAsync(request);
                if (result.Succeeded)
                {
                    try
                    {
                        var titleAr = Request.Form["Title"].ToString();
                        var titleEn = Request.Form["EnglishTitle"].ToString();
                        if (string.IsNullOrWhiteSpace(titleEn)) titleEn = titleAr;
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Title", titleAr, "ar");
                        AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Title", titleEn, "en");

                        var descAr = Request.Form["Description"].ToString();
                        var descEn = Request.Form["EnglishDescription"].ToString();
                        if (!string.IsNullOrWhiteSpace(descAr) || !string.IsNullOrWhiteSpace(descEn))
                        {
                            if (string.IsNullOrWhiteSpace(descEn)) descEn = descAr;
                            if (string.IsNullOrWhiteSpace(descAr)) descAr = descEn;
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Description", descAr ?? string.Empty, "ar");
                            AseerAlkotb.Localization.Resources.ResxResourceHelper.UpsertSharedResource($"Book_{id}_Description", descEn ?? string.Empty, "en");
                        }
                    }
                    catch { }
                    TempData["Success"] = "Book updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = result.Message;
            }
            return View(request);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(id));
            var author = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(book.Data.AuthorId));
            var publisher = await _publisherServices.GetPublisherByIdAsync(new GetPublisherByIdRequest(book.Data.PublisherId));
            ViewBag.Author = author.Data;
            ViewBag.Publisher = publisher.Data;


            return View("DeleteBook", book.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookServices.DeleteBookAsync(new DeleteBookRequest(id));
            return RedirectToAction(nameof(Index));
        }

        // ==================== AJAX ENDPOINTS ====================

        [HttpGet]
        public async Task<IActionResult> SearchCategories(string term)
        {
            var categories = await _categoryServices.GetAllCategoriesPaginatedAsync(
                new GetAllCategoriesPaginatedRequest { Search = term, PageSize = 10 });

            var parents = categories.Data.Select(c => new { id = c.Id, text = c.Name });

            return Json(parents);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAuthors(string term)
        {
            var authors = await _authorServices.GetAllAuthorsPaginatedAsync(
                new GetAllAuthorsPaginatedRequest { Search = term, PageSize = 10 });

            return Json(authors.Data.Select(a => new { id = a.Id, text = a.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> SearchPublishers(string term)
        {
            var publishers = await _publisherServices.GetAllPublishersPaginatedAsync(
                new GetAllPublishersPaginatedRequest { Search = term, PageSize = 10 });

            return Json(publishers.Data.Select(p => new { id = p.Id, text = p.Name }));
        }
       

    }
}