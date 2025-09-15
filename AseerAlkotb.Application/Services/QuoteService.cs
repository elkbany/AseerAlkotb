using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Quotes.Requests;
using AseerAlkotb.Application.Features.Quotes.Responses;
using AseerAlkotb.Application.Features.Quotes.Validators;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class QuoteService : AppService, IQuoteService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public QuoteService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IServiceProvider serviceProvider, IHostEnvironment environment, IUnitOfWork unitOfWork) : base(serviceProvider, environment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddQuoteResponse>> AddQuoteAsync(AddQuoteRequest request)
        {
            await DoValidationAsync<AddQuoteRequestValidator, AddQuoteRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<AddQuoteResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<AddQuoteResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<AddQuoteResponse>();
            }

            if (request.BookId.HasValue)
            {
                var book = await unitOfWork.Books.AnyAsync(b => b.Id == request.BookId.Value);
                if (!book)
                {
                    return BadRequest<AddQuoteResponse>($"{_stringLocalizer["Book"]} {_stringLocalizer["NotFound"]}");
                }
            }
            else if (request.AuthorId.HasValue)
            {
                var author = await unitOfWork.Authors.AnyAsync(a => a.Id == request.AuthorId.Value);
                if (!author)
                {
                    return BadRequest<AddQuoteResponse>($"{_stringLocalizer["Author"]} {_stringLocalizer["NotFound"]}");
                }
            }

            var quote = request.Adapt<Quote>();
            quote.QuoteFor = request.BookId.HasValue ? QuoteFor.Book : QuoteFor.Author;
            quote.UserId = currentUser.Id; // Set the user who created the quote

            await unitOfWork.Quotes.InsertAsync(quote);
            await unitOfWork.CommitAsync();

            var response = quote.Adapt<AddQuoteResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<UpdateQuoteResponse>> UpdateQuoteAsync(UpdateQuoteRequest request)
        {
            await DoValidationAsync<UpdateQuoteRequestValidator, UpdateQuoteRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<UpdateQuoteResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<UpdateQuoteResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<UpdateQuoteResponse>();
            }

            var quote = await unitOfWork.Quotes.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (quote == null)
            {
                return NotFound<UpdateQuoteResponse>($"{_stringLocalizer["Quote"]} {_stringLocalizer["NotFound"]}");
            }

            // Check if the current user owns this quote
            if (quote.UserId != currentUser.Id)
            {
                return UnAuthorized<UpdateQuoteResponse>();
            }

            request.Adapt(quote);
            unitOfWork.Quotes.Update(quote);
            await unitOfWork.CommitAsync();

            var response = quote.Adapt<UpdateQuoteResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<DeleteQuoteResponse>> DeleteQuoteAsync(DeleteQuoteRequest request)
        {
            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<DeleteQuoteResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<DeleteQuoteResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<DeleteQuoteResponse>();
            }

            var quote = await unitOfWork.Quotes.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (quote == null)
            {
                return NotFound<DeleteQuoteResponse>($"{_stringLocalizer["Quote"]} {_stringLocalizer["NotFound"]}");
            }

            // Check if the current user owns this quote
            if (quote.UserId != currentUser.Id)
            {
                return UnAuthorized<DeleteQuoteResponse>();
            }

            unitOfWork.Quotes.Delete(quote);
            await unitOfWork.CommitAsync();

            var response = quote.Adapt<DeleteQuoteResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<GetQuoteByIdResponse>> GetQuoteByIdAsync(GetQuoteByIdRequest request)
        {
            var quote = await unitOfWork.Quotes
                .FirstOrDefaultAsync(
                    q => q.Id == request.Id,
                    default,
                    r => r.Book,
                    r => r.Author,
                    r => r.User
                );

            if (quote == null)
            {
                return NotFound<GetQuoteByIdResponse>($"{_stringLocalizer["Quote"]} {_stringLocalizer["NotFound"]}");
            }

            var response = new GetQuoteByIdResponse
            (
                quote.Id,
                quote.BookId,
                quote.AuthorId,
                quote.UserId,
                quote.User.FirstName + ' ' + quote.User.LastName,
                quote.Comment ?? string.Empty
            );
            return Success(response);
        }

        public async Task<ApiResponse<List<GetAllQuotesPaginatedResponse>>> GetAllQuotesAsync(GetAllQuotesPaginatedRequest request)
                
        {
            var quotes = await unitOfWork.Quotes.GetAllAsync(
                    r => (request.BookId.HasValue && r.BookId == request.BookId.Value) ||
                         (request.AuthorId.HasValue && r.AuthorId == request.AuthorId.Value),
                         (request.PageNumber - 1) * request.PageSize, request.PageSize,
                    default,
                    r => r.Book,
                    r => r.Author,
                    r => r.User
                );

            var totalCount = quotes.Count;

            var response = quotes.Select(q => new GetAllQuotesPaginatedResponse
            (
                q.Id,
                q.BookId,
                q.AuthorId,
                q.UserId,
                q.User.FirstName + ' ' + q.User.LastName,
                q.Comment ?? string.Empty
            )).ToList();

            return Success(response, totalCount, request.PageNumber, request.PageSize);
        }

      
    }
}