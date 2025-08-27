using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Reviews.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookServices _bookServices;
        private readonly IAuthorServices _authorServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IPublisherServices _publisherServices;
        private readonly IReviewServices _reviewServices; // Add this

        public BooksController(
            IBookServices bookServices,
            IAuthorServices authorServices,
            ICategoryServices categoryServices,
            IPublisherServices publisherServices,
            IReviewServices reviewServices) // Add this parameter
        {
            _bookServices = bookServices;
            _authorServices = authorServices;
            _categoryServices = categoryServices;
            _publisherServices = publisherServices;
            _reviewServices = reviewServices; // Add this
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string Search = "")
        {
            var request = new GetAllBooksPaginatedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = Search
            };

            var result = await _bookServices.GetAllBooksPaginatedAsync(request);

            if (result != null)
            {
                ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.SearchTerm = Search;
            }

            return View(result);
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

            var currentBook = await _bookServices.GetBookByIdAsync(new GetBookByIdRequest(request.Id));
            string? currentCoverImageUrl = currentBook.Succeeded && currentBook.Data != null
                ? currentBook.Data.CoverImageUrl
                : string.Empty;

            var model = new UpdateBookResponse(
                request.Id,
                request.Title,
                request.ISBN,
                request.Price,
                request.Description,
                request.DiscountPercentage,
                request.PublishedDate,
                request.PageCount,
                request.Language,
                currentCoverImageUrl,
                request.Format,
                request.StockQuantity,
                request.AuthorId,
                request.PublisherId,
                request.CategoryIds,
                request.IsActive
            );

            return View(model);
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

            var parents = categories.Data
                   .Where(c => c.ParentCategoryId == null)
                   .Select(c => new { id = c.Id, text = c.Name });

            return Json(parents);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSubCategories(int categoryId, string? term)
        {
            var categories = await _categoryServices.GetAllCategoriesPaginatedAsync(
               new GetAllCategoriesPaginatedRequest { Search = term, PageSize = 10 });

            var subCategories = await _categoryServices.GetAllSubCategoriesPaginatedAsync(
                new GetAllSubCategoriesPaginatedRequest(categoryId, 1, 10, term));

            var subs = subCategories.Data.Select(sc => new { id = sc.Id, text = sc.Name });

            return Json(subs);
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