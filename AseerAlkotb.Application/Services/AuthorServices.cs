﻿﻿

using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.Features.Publishers.Validators;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Localization.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class AuthorServices : AppService,IAuthorServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<User> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorServices(IUnitOfWork unitOfWork , IServiceProvider serviceProvider,IHostEnvironment environment, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : base(serviceProvider,environment) 
        {
            this.unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

        }
        public async Task<ApiResponse<AddAuthorResponse>> AddAuthorAsync(AddAuthorRequest request)
        {
            await DoValidationAsync<AddAuthorRequestValidator, AddAuthorRequest>(request);
           
            var author = request.Adapt<Author>();
            if (request.Image != null)
            {
                author.ImageUrl = (await UploadImageAsync(request.Image, "authors")).CloudUrl;
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
                return NotFound<DeleteAuthorResponse>($"{_stringLocalizer["Author"]} {_stringLocalizer["NotFound"]}");
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
                return NotFound<UpdateAuthorResponse>($"{_stringLocalizer["Author"]} {_stringLocalizer["NotFound"]}");
            }
            request.Adapt(author); 
            if (request.Image != null)
            {
                var uploadResult = await UpdateImageAsync(
                   request.Image,
                   author.ImageUrl ?? string.Empty,
                   "authors"
               );

                author.ImageUrl = !string.IsNullOrEmpty(uploadResult.CloudUrl) ? uploadResult.CloudUrl : uploadResult.LocalUrl;
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
                return NotFound<GetAuthorByIdResponse>($"{_stringLocalizer["Author"]} {_stringLocalizer["NotFound"]}");
            }
            var authMap = author.Adapt<GetAuthorByIdResponse>();
            authMap.Name = GetLocalizedText(author.Name, author.Name_en);
            authMap.Bio = GetLocalizedText(author.Bio, author.Bio_en);
            authMap.Rating=author.Reviews?.Any() == true ? (decimal)author.Reviews.Average(r => r.Rating) : 0;
            authMap.Books = author.Books.Select(book =>
            {
                var bookDto = book.Adapt<BookCardDto>();
                bookDto.Title = GetLocalizedText(book.Title, book.Title_en);
                bookDto.Rating = book.Reviews?.Any() == true ? (decimal)book.Reviews.Average(r => r.Rating): 0;
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
                authorDto.Name = GetLocalizedText(author.Name, author.Name_en);
                authorDto.Rating = author.Reviews?.Any() == true ? (decimal)author.Reviews.Average(r => r.Rating): 0;
                authorDto.Books = author.Books.Select(book =>
                {
                    var bookDto = book.Adapt<BookCardDto>();
                    bookDto.Title = GetLocalizedText(book.Title, book.Title_en);
                    bookDto.Rating = book.Reviews?.Any() == true ? (decimal)book.Reviews.Average(r => r.Rating): 0;
                    return bookDto;
                }).ToList();
                return authorDto;
            }).ToList();

            return Success(authsMap, totalCount, request.PageNumber, request.PageSize);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<ApiResponse<FollowAutherResponse>>FollowAuther(FollowAutherRequest request)
        {
            await DoValidationAsync<FollowAuthorRequestValidation, FollowAutherRequest>(request);

            // Get current user from HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<FollowAutherResponse>();
            }

            var currentUser = await _userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<FollowAutherResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await _userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<FollowAutherResponse>();
            }

            if (await unitOfWork.Authors.IsFollowingAuther(currentUser.Id, request.authorId)) { 
                return BadRequest<FollowAutherResponse>("You are already following this author");
            }
            else
            {
                var userFollow=await unitOfWork.Authors.FollowAuther(currentUser.Id, request.authorId);
                await unitOfWork.CommitAsync();
                var response=userFollow.Adapt<FollowAutherResponse>();
                return Success(response);
            }
        }

        public async Task<ApiResponse<UnFollowAuthorResponse>>UnFollowAuthor(UnFollowAuthorRequest request)
        {
            await DoValidationAsync<UnFollowAuthorRequestValidation, UnFollowAuthorRequest>(request);
            // Get current user from HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<UnFollowAuthorResponse>();
            }

            var currentUser = await _userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<UnFollowAuthorResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await _userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<UnFollowAuthorResponse>();
            }
            if (await unitOfWork.Authors.IsFollowingAuther(currentUser.Id,request.authorId))
            {
                var userFollow = await unitOfWork.Authors.UnFollowAuther(currentUser.Id, request.authorId);
                await unitOfWork.CommitAsync();
                var response = userFollow.Adapt<UnFollowAuthorResponse>();
                return Success(response);
            }
            else
            {
                return BadRequest<UnFollowAuthorResponse>("User is not following this author");
            }
        }

        public async Task<ApiResponse<GetAutherFollowerCountResponse>> GetAutherFollowerCount(GetAutherFollowerCountRequest request)
        {
           await DoValidationAsync<GetAutherFollowerCountRequestValidation, GetAutherFollowerCountRequest>(request);
            var count = await unitOfWork.Authors.GetAuthorFollowerCount(request.AuthorId);
            var response = new GetAutherFollowerCountResponse()
            {
                AuthorId = request.AuthorId,
                FollowerCount = count
            };
            return Success(response);
        }

        public async Task<ApiResponse<List<GetFollowedAuthorResponse>>> GetFollowedAuther(GetFollowedAuthorRequest request)
        {
            await DoValidationAsync<GetFollowedAuthorRequestValidatin, GetFollowedAuthorRequest>(request);
            var Authors = unitOfWork.Authors.GetFollowedAuther(request.UserId).ToList();
            var response = Authors.Adapt<List<GetFollowedAuthorResponse>>();
            return Success(response);
            
        }

        public async Task<ApiResponse<List<GetFollowersAuthorResponse>>> GetFollowerAuther(GetFollowersAuthorRequest request)
        {
            await DoValidationAsync<GetFollowersAuthorRequestValidation, GetFollowersAuthorRequest>(request);
            var Authors = unitOfWork.Authors.GetFollowerAuther(request.AuthorId).ToList();
            var response = Authors.Adapt<List<GetFollowersAuthorResponse>>();
            return Success(response);

        }

        public async Task<ApiResponse<IsFollowingResponse>> IsFollowing(IsFollowingAuthorRequest request)
        {
            //await DoValidationAsync<IsFollowingAuthorRequestValidator, IsFollowingAuthorRequest>(request);
            // Get current user from HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<IsFollowingResponse>();
            }

            var currentUser = await _userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<IsFollowingResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await _userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<IsFollowingResponse>();
            }
            var result = await unitOfWork.Authors.IsFollowingAuther(currentUser.Id, request.authorId);
            if (result)
            {
                var isFollowingResponse = new IsFollowingResponse
                (
                     isFollow: true
                );
                return Success(isFollowingResponse);
            }
            else
            {
                var isFollowingResponse = new IsFollowingResponse
               (
                    isFollow: false
               );
                return Success(isFollowingResponse);

            }

        }




    }
}
