
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class QuoteService : AppService , IQuoteService
    {
        private readonly IUnitOfWork unitOfWork;

        public QuoteService(IServiceProvider serviceProvider, IHostEnvironment environment, IUnitOfWork unitOfWork) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddQuoteResponse>> AddQuoteAsync(AddQuoteRequest request)
        {
            await DoValidationAsync<AddQuoteRequestValidator, AddQuoteRequest>(request);

            if (request.BookId.HasValue)
            {
                var book = await unitOfWork.Books.AnyAsync(b => b.Id == request.BookId.Value);
                if (!book)
                {
                    return BadRequest<AddQuoteResponse>("Book not found");
                }
            }

            else if (request.AuthorId.HasValue)
            {
                var author = await unitOfWork.Authors.AnyAsync(a => a.Id == request.AuthorId.Value);
                if (!author)
                {
                    return BadRequest<AddQuoteResponse>("Author not found");
                }
            }
            var quote = request.Adapt<Quote>();
            quote.QuoteFor = request.BookId.HasValue ? QuoteFor.Book : QuoteFor.Author;

            await unitOfWork.Quotes.InsertAsync(quote);

            await unitOfWork.CommitAsync();

            var response = quote.Adapt<AddQuoteResponse>();

            return Success(response);
        }

        public async Task<ApiResponse<UpdateQuoteResponse>> UpdateQuoteAsync(UpdateQuoteRequest request)
        {
            await DoValidationAsync<UpdateQuoteRequestValidator, UpdateQuoteRequest>(request);
            var quote = await unitOfWork.Quotes.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (quote == null)
            {
                return NotFound<UpdateQuoteResponse>("Quote not found");
            }
            request.Adapt(quote);
            unitOfWork.Quotes.Update(quote);
            await unitOfWork.CommitAsync();
            var response = quote.Adapt<UpdateQuoteResponse>();
            return Success(response);
        }


        public async Task<ApiResponse<DeleteQuoteResponse>> DeleteQuoteAsync(DeleteQuoteRequest request)
        {
            var quote = await unitOfWork.Quotes.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (quote == null)
            {
                return NotFound<DeleteQuoteResponse>("Quote not found");
            }
            unitOfWork.Quotes.Delete(quote);
            await unitOfWork.CommitAsync();
            var response = quote.Adapt<DeleteQuoteResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<GetQuoteByIdResponse>> GetQuoteByIdAsync(GetQuoteByIdRequest request)
        {
            var quote = await unitOfWork.Quotes.FirstOrDefaultAsync(q => q.Id == request.Id , default , r => r.Book, r => r.Author , r => r.User);
            if (quote == null)
            {
                return NotFound<GetQuoteByIdResponse>("Quote not found");
            }
            var response = quote.Adapt<GetQuoteByIdResponse>();
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

            var response = quotes.Adapt<List<GetAllQuotesPaginatedResponse>>();
            return Success(response, totalCount, request.PageNumber, request.PageSize);
        }

    }
}
