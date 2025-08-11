using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.Features.Reviews.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class ReviewServices : AppService,IReviewServices
    {
        private readonly IUnitOfWork unitOfWork;

        public ReviewServices(IServiceProvider serviceProvider, IHostEnvironment environment,IUnitOfWork unitOfWork) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<AddReviewResponse>> AddReviewAsync(AddReviewRequest request)
        {
            await DoValidationAsync<AddReviewRequestValidator, AddReviewRequest>(request);

            if (request.BookId.HasValue)
            {
                var bookExists = await unitOfWork.Books.AnyAsync(b => b.Id == request.BookId.Value);
                if (!bookExists)
                {
                    return BadRequest<AddReviewResponse>("Book not found.");
                }   
            }
            else if (request.AuthorId.HasValue)
            {
                var authorExists = await unitOfWork.Authors.AnyAsync(a => a.Id == request.AuthorId.Value);
                if (!authorExists)
                {
                    return BadRequest<AddReviewResponse>("Author not found.");
                }
            }     
            var review = request.Adapt<Review>();
            review.ReviewFor = request.BookId.HasValue ? ReviewFor.Book : ReviewFor.Author;
            await unitOfWork.Reviews.InsertAsync(review);
            await unitOfWork.CommitAsync();
            var revMap = review.Adapt<AddReviewResponse>();
            return Success(revMap);
        }
        public async Task<ApiResponse<UpdateReviewResponse>> UpdateReviewAsync(UpdateReviewRequest request)
        {
            await DoValidationAsync<UpdateReviewRequestValidator, UpdateReviewRequest>(request);

            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            if (review == null)
            {
                return NotFound<UpdateReviewResponse>("Review not found.");
            }
            request.Adapt(review);
            unitOfWork.Reviews.Update(review);
            await unitOfWork.CommitAsync();

            var revMap= review.Adapt<UpdateReviewResponse>();
            return Success(revMap);
        }
        public async Task<ApiResponse<DeleteReviewResponse>> DeleteReviewAsync(DeleteReviewRequest request)
        {
            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            if (review == null)
            {
                return NotFound<DeleteReviewResponse>("Review not found.");
            }

             unitOfWork.Reviews.Delete(review);
            await unitOfWork.CommitAsync();
            var revMap = review.Adapt<DeleteReviewResponse>();
            return Success(revMap);
        }

        public async Task<ApiResponse<GetReviewByIdResponse>> GetReviewByIdAsync(GetReviewByIdRequest request)
        {
            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(
                    r => r.Id == request.Id,
                    default,
                    r => r.Book,
                    r => r.Author,
                    r => r.User
                );

            if (review == null)
            {
                return NotFound<GetReviewByIdResponse>("Review not found.");
            }

            var revMap = review.Adapt<GetReviewByIdResponse>();
            return Success(revMap);
        }
        public async Task<ApiResponsePaginated<List<GetAllReviewsPaginatedResponse>>> GetAllReviewsAsync(GetAllReviewsPaginatedRequest request)
        {
            var reviews = await unitOfWork.Reviews
                .GetAllAsync(
                    r => (request.BookId.HasValue && r.BookId == request.BookId.Value) ||
                         (request.AuthorId.HasValue && r.AuthorId == request.AuthorId.Value),
                         (request.PageNumber - 1) * request.PageSize, request.PageSize,
                    default,
                    r => r.Book,
                    r => r.Author,
                    r => r.User
                );

            var totalCount =  reviews.Count;
            var revMap = reviews.Adapt<List<GetAllReviewsPaginatedResponse>>();
            return Success(revMap, totalCount, request.PageNumber, request.PageSize);

        }


    }
}
