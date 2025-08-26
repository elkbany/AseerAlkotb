using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookServices _bookServices;
        private readonly IAuthorServices _authorServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IPublisherServices _publisherServices;

        public BooksController(
            IBookServices bookServices,
            IAuthorServices authorServices,
            ICategoryServices categoryServices,
            IPublisherServices publisherServices)
        {
            _bookServices = bookServices;
            _authorServices = authorServices;
            _categoryServices = categoryServices;
            _publisherServices = publisherServices;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _bookServices.GetAllBooksPaginatedAsync(new GetAllBooksPaginatedRequest());
            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(id));

            if (result == null || result.Data == null)
                return NotFound("Book not found");

            if (result.Data.PublisherId == 0)
                return BadRequest("Book has no publisher assigned");

            var author = await _authorServices.GetAuthorByIdAsync(new GetAuthorByIdRequest(result.Data.AuthorId));
            var publisher = await _publisherServices.GetPublisherByIdAsync(new GetPublisherByIdRequest(result.Data.PublisherId));

            if (publisher == null || publisher.Data == null)
                return NotFound("Publisher not found");

            // استرجاع التصنيفات المرتبطة بالكتاب
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

            return View(result.Data);
        }

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
                await _bookServices.AddBookAsync(request);
                return RedirectToAction(nameof(Index));
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBookRequest request)
        {
            if (ModelState.IsValid)
            {
                await _bookServices.UpdateBookAsync(request);
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(id));
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

            return Json(categories.Data.Select(c => new { id = c.Id, text = c.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(int categoryId, string? term)
        {
            var subCategories = await _categoryServices.GetAllSubCategoriesPaginatedAsync(
                new GetAllSubCategoriesPaginatedRequest(1, 10, categoryId, term));

            return Json(subCategories.Data.Select(sc => new { id = sc.Id, text = sc.Name }));
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
