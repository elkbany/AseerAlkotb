using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Books.Responses;
using AseerAlkotb.Application.Features.Books.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
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
            var authMap = books.Adapt<List<GetAllBooksPaginatedResponse>>();
            return Success(authMap, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
