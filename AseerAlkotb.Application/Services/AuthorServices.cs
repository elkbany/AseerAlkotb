

using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class AuthorServices : AppService,IAuthorServices
    {
        private readonly IUnitOfWork unitOfWork;

        public AuthorServices(IUnitOfWork unitOfWork , IServiceProvider serviceProvider,IHostEnvironment environment) : base(serviceProvider,environment) 
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<AddAuthorResponse>> AddAuthorAsync(AddAuthorRequest request)
        {
            await DoValidationAsync<AddAuthorRequestValidator, AddAuthorRequest>(request);
           
            var author = request.Adapt<Author>();
            if (request.Image != null)
            {
                author.ImageUrl = await UploadImageAsync(request.Image, "authors");
            }
            await unitOfWork.Authors.InsertAsync(author);
            await unitOfWork.CommitAsync();

            var authMap = author.Adapt<AddAuthorResponse>();
            return Success(authMap);
        }
        public async Task<ApiResponse<DeleteAuthorResponse>> DeleteAuthorAsync(DeleteAuthorRequest request)
        {
            await DoValidationAsync<DeleteAuthorRequestValidator, DeleteAuthorRequest>(request);
            var author = await unitOfWork.Authors.FirstOrDefaultAsync(a=>a.Id==request.Id);
            if (author==null)
            {
                return NotFound<DeleteAuthorResponse>("Author not found");
            }
            if (!string.IsNullOrEmpty(author.ImageUrl))
            {
                await DeleteImageAsync(author.ImageUrl);
            }
            unitOfWork.Authors.Delete(author);
            await unitOfWork.CommitAsync();
            var authMap= author.Adapt<DeleteAuthorResponse>();
            return Success(authMap);
          
        }
        public async Task<ApiResponse<UpdateAuthorResponse>> UpdateAuthorAsync(UpdateAuthorRequest request)
        {
            await DoValidationAsync<UpdateAuthorRequestValidator, UpdateAuthorRequest>(request);
            var author = await unitOfWork.Authors.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (author == null)
            {
                return NotFound<UpdateAuthorResponse>("Auhtor not found");
            }
            request.Adapt(author); 
            if (request.Image != null)
            {
                author.ImageUrl = await UpdateImageAsync(
                    request.Image,
                    author.ImageUrl,
                    "authors"
               );
            }
                unitOfWork.Authors.Update(author);
            await unitOfWork.CommitAsync();
           var authMap = author.Adapt<UpdateAuthorResponse>();
            return Success(authMap);
        }
        public async Task<ApiResponse<GetAuthorByIdResponse>> GetAuthorByIdAsync(GetAuthorByIdRequest request)
        {
            await DoValidationAsync<GetAuthorByIdRequestValidator, GetAuthorByIdRequest>(request);
            var author = await unitOfWork.Authors.FirstOrDefaultAsync(a => a.Id == request.Id,default,a=>a.Books,a=>a.Reviews);

            if (author == null)
            {
                return NotFound<GetAuthorByIdResponse>("Auhtor not found");
            }
            var authMap = author.Adapt<GetAuthorByIdResponse>();
            authMap.Rating=(int)Math.Round(author.Reviews.Average(a=>a.Rating));
            authMap.Books = author.Books.Select(book =>
            {
                var bookDto = book.Adapt<BookCardDto>();
                bookDto.Rating = book.Reviews?.Any() == true ? (int)Math.Round(book.Reviews.Average(r => r.Rating)) : 0;
                return bookDto;
            }).ToList();
            return Success(authMap);
        }
        public async Task<ApiResponsePaginated<List<GetAllAuthorsPaginatedResponse>>> GetAllAuthorsPaginatedAsync(GetAllAuthorsPaginatedRequest request)
        {
            await DoValidationAsync<GetAllAuthorsPaginatedRequestValidator, GetAllAuthorsPaginatedRequest>(request);
            var authors = await unitOfWork.Authors
                .GetAllAsync(s => s.Name.Contains(request.Search), 
                (request.PageNumber - 1) * request.PageSize, request.PageSize,default,a=>a.Books,a=>a.Reviews);
            var totalCount = await unitOfWork.Authors.CountAsync((s => s.Name.Contains(request.Search)));
            var authsMap = authors.Select(author =>
            {
                var authorDto = author.Adapt<GetAllAuthorsPaginatedResponse>();
                authorDto.Rating = author.Reviews?.Any() == true ? (int)Math.Round(author.Reviews.Average(r => r.Rating)) : 0;
                authorDto.Books = author.Books.Select(book =>
                {
                    var bookDto = book.Adapt<BookCardDto>();
                    bookDto.Rating = book.Reviews?.Any() == true ? (int)Math.Round(book.Reviews.Average(r => r.Rating)) : 0;
                    return bookDto;
                }).ToList();
                return authorDto;
            }).ToList();

            return Success(authsMap, totalCount, request.PageNumber, request.PageSize);

        }

    }
}
