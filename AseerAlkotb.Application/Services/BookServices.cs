using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
using AseerAlkotb.Application.Features.Books.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;



namespace AseerAlkotb.Application.Services
{
    public class BookServices : AppService, IBookServices
    {
        private readonly IUnitOfWork _uniteOfWork;

        public BookServices(IUnitOfWork uniteOfWork, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment) : base(serviceProvider, hostEnvironment)
        {
            _uniteOfWork = uniteOfWork;
        }


        public async Task<ApiResponse<AddBookResponse>> AddBookAsync(AddBookRequest request)
        {
            await DoValidationAsync<AddBookRequestValidator, AddBookRequest>(request);

            var book = request.Adapt<Book>();
            if (request.CoverImageUrl != null)
            {
                book.CoverImageUrl = await UploadImageAsync(request.CoverImageUrl, "Books");
            }

            await _uniteOfWork.Books.InsertAsync(book);
            await _uniteOfWork.CommitAsync();

            var bookMap = book.Adapt<AddBookResponse>();
            return Success(bookMap);
        }

        public async Task<ApiResponse<UpdateBookResponse>> UpdateBookAsync(UpdateBookRequest request)
        {
            await DoValidationAsync<UpdateBookRequestValidator, UpdateBookRequest>(request);

            var book = await _uniteOfWork.Books.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (book == null)
            {
                return NotFound<UpdateBookResponse>("Book not found");
            }

            request.Adapt(book);

            if (request.CoverImageUrl != null)
            {
                if (!string.IsNullOrEmpty(book.CoverImageUrl))
                {
                    await DeleteImageAsync(book.CoverImageUrl);
                }
                book.CoverImageUrl = await UploadImageAsync(request.CoverImageUrl, "Books");
            }

            _uniteOfWork.Books.Update(book);

            await _uniteOfWork.CommitAsync();

            var bookMap = book.Adapt<UpdateBookResponse>();
            return Success(bookMap);
        }
        public async Task<ApiResponse<DeleteBookResponse>> DeleteBookAsync(DeleteBookRequest request)
        {
            await DoValidationAsync<DeleteBookRequestValidator, DeleteBookRequest>(request);
            var book = await _uniteOfWork.Books.FirstOrDefaultAsync(b => b.Id == request.Id);
            if (book == null)
            {
                return NotFound<DeleteBookResponse>("Book not found");
            }
            if (!string.IsNullOrEmpty(book.CoverImageUrl))
            {
                await DeleteImageAsync(book.CoverImageUrl);
            }

            _uniteOfWork.Books.Delete(book);
            await _uniteOfWork.CommitAsync();
            var bookMap = book.Adapt<DeleteBookResponse>();
            return Success(bookMap);
        }

        public async Task<ApiResponse<GetBookByIdResponse>> GetBookByIdAsync(GetBookByIdRequest request)
        {
            await DoValidationAsync<GetBookByIdRequestValidator, GetBookByIdRequest>(request);
            var book = await _uniteOfWork.Books.FirstOrDefaultAsync(b => b.Id == request.Id);
            if (book == null)
            {
                return NotFound<GetBookByIdResponse>("Book not found");
            }
            var bookMap = book.Adapt<GetBookByIdResponse>();
            return Success(bookMap);
        }
     
    
    public async Task<ApiResponsePaginated<List<GetAllBooksPaginatedResponse>>> GetAllBooksPaginatedAsync(GetAllBooksPaginatedRequest request)
    {
        await DoValidationAsync<GetAllBooksPaginatedValidator, GetAllBooksPaginatedRequest>(request);
        var books = await _uniteOfWork.Books
                .GetAllAsync(s => s.Title.Contains(request.Search),
            (request.PageNumber - 1) * request.PageSize, request.PageSize);

            var totalCount = await _uniteOfWork.Books.CountAsync(s => s.Title.Contains(request.Search));
            var bookMap = books.Adapt<List<GetAllBooksPaginatedResponse>>();
            return Success(bookMap, totalCount, request.PageNumber, request.PageSize);
    }
        public async Task<ApiResponsePaginated<List<GetAllBooksPaginatedResponse>>> FilterBooksAsync(FilterBooksRequest request)
        {
            var query = _uniteOfWork.Books.GetQueryable()
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .AsNoTracking();

            // فلتر حسب الكلمة
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(b => b.Title.Contains(request.SearchTerm) || b.Description.Contains(request.SearchTerm));

            // فلتر اللغة
            if (request.Language is not null)
                query = query.Where(b => b.Language == request.Language.Value);


            // فلتر التصنيفات
            if (request.CategoryIds is not null && request.CategoryIds.Any())
                query = query.Where(b => b.Categories.Any(c => request.CategoryIds.Contains(c.Id)));

            // فلتر الناشرين
            query = query.Where(b => b.PublisherId.HasValue && request.PublisherIds.Contains(b.PublisherId.Value));


            // الترتيب
            query = request.SortBy switch
            {
                BookSortOption.Newest => query.OrderByDescending(b => b.PublishedDate),
                BookSortOption.Oldest => query.OrderBy(b => b.PublishedDate),
                BookSortOption.PriceAsc => query.OrderBy(b => b.Price),
                BookSortOption.PriceDesc => query.OrderByDescending(b => b.Price),
                BookSortOption.MostPopular => query.OrderByDescending(b => b.ViewCount),
                _ => query.OrderByDescending(b => b.CreatedAt) // fallback في حالة null
            };

            var totalCount = await query.CountAsync();

            var paginatedBooks = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var response = paginatedBooks.Adapt<List<GetAllBooksPaginatedResponse>>();

            return Success(
                response, totalCount, request.PageNumber, request.PageSize
            );
        }



    }
}
