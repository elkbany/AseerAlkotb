using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Reviews.Requests;
using AseerAlkotb.Application.Features.Reviews.Responses;
using AseerAlkotb.Application.Features.Reviews.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Services
{
    public class ReviewServices : AppService, IReviewServices
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public ReviewServices(IHttpContextAccessor httpContextAccessor, Microsoft.AspNetCore.Identity.UserManager<User> userManager, IServiceProvider serviceProvider, IHostEnvironment environment, IUnitOfWork unitOfWork) : base(serviceProvider, environment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddReviewResponse>> AddReviewAsync(AddReviewRequest request)
        {
            await DoValidationAsync<AddReviewRequestValidator, AddReviewRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<AddReviewResponse>();
            }

            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<AddReviewResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<AddReviewResponse>();
            }

            if (request.BookId.HasValue)
            {
                var bookExists = await unitOfWork.Books.AnyAsync(b => b.Id == request.BookId.Value);
                if (!bookExists)
                {
                    return BadRequest<AddReviewResponse>($"{_stringLocalizer["Book"]} {_stringLocalizer["NotFound"]}");
                }
            }
            else if (request.AuthorId.HasValue)
            {
                var authorExists = await unitOfWork.Authors.AnyAsync(a => a.Id == request.AuthorId.Value);
                if (!authorExists)
                {
                    return BadRequest<AddReviewResponse>($"{_stringLocalizer["Author"]} {_stringLocalizer["NotFound"]}");
                }
            }

            // Check if user already reviewed this item (optional business rule)
            var existingReview = await unitOfWork.Reviews.AnyAsync(r =>
                r.UserId == currentUser.Id &&
                ((request.BookId.HasValue && r.BookId == request.BookId.Value) ||
                 (request.AuthorId.HasValue && r.AuthorId == request.AuthorId.Value)));

            if (existingReview)
            {
                return BadRequest<AddReviewResponse>($"{_stringLocalizer["Review"]} {_stringLocalizer["AlreadyExists"]}");
            }

            var review = request.Adapt<Review>();
            review.ReviewFor = request.BookId.HasValue ? ReviewFor.Book : ReviewFor.Author;
            review.UserId = currentUser.Id; // Set the user who created the review

            await unitOfWork.Reviews.InsertAsync(review);
            await unitOfWork.CommitAsync();

            var revMap = review.Adapt<AddReviewResponse>();
            return Success(revMap);
        }

        public async Task<ApiResponse<UpdateReviewResponse>> UpdateReviewAsync(UpdateReviewRequest request)
        {
            await DoValidationAsync<UpdateReviewRequestValidator, UpdateReviewRequest>(request);

            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<UpdateReviewResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<UpdateReviewResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<UpdateReviewResponse>();
            }

            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.Id);
            if (review == null)
            {
                return NotFound<UpdateReviewResponse>($"{_stringLocalizer["Review"]} {_stringLocalizer["NotFound"]}");
            }

            // Check if the current user owns this review
            if (review.UserId != currentUser.Id)
            {
                return UnAuthorized<UpdateReviewResponse>();
            }

            request.Adapt(review);
            review.UpdatedAt = DateTime.UtcNow; // Optional: track when updated
            unitOfWork.Reviews.Update(review);
            await unitOfWork.CommitAsync();

            var revMap = review.Adapt<UpdateReviewResponse>();
            return Success(revMap);
        }

        public async Task<ApiResponse<DeleteReviewResponse>> DeleteReviewAsync(DeleteReviewRequest request)
        {
            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<DeleteReviewResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<DeleteReviewResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<DeleteReviewResponse>();
            }

            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.Id);
            if (review == null)
            {
                return NotFound<DeleteReviewResponse>($"{_stringLocalizer["Review"]} {_stringLocalizer["NotFound"]}");
            }

            // Check if the current user owns this review
            if (review.UserId != currentUser.Id)
            {
                return UnAuthorized<DeleteReviewResponse>();
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
                return NotFound<GetReviewByIdResponse>($"{_stringLocalizer["Review"]} {_stringLocalizer["NotFound"]}");
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

            var totalCount = reviews.Count;
            var revMap = reviews.Adapt<List<GetAllReviewsPaginatedResponse>>();
            return Success(revMap, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<ApiResponse<GetReviewByIdResponse>> LikeReviewAsync(LikeReviewRequest request)
        {
            // Get current user from HttpContext
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return UnAuthorized<GetReviewByIdResponse>();
            }
            var currentUser = await userManager.GetUserAsync(httpContext.User);
            if (currentUser == null)
            {
                return UnAuthorized<GetReviewByIdResponse>();
            }

            // Check if user has "Client" role
            var isInClientRole = await userManager.IsInRoleAsync(currentUser, "Client");
            if (!isInClientRole)
            {
                return UnAuthorized<GetReviewByIdResponse>();
            }

            var review = await unitOfWork.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, default, r => r.LikeDisLikes);
            if (review == null)
                return NotFound<GetReviewByIdResponse>($"{_stringLocalizer["Review"]} {_stringLocalizer["NotFound"]}");

            // Optional: Prevent users from liking their own reviews
            if (review.UserId == currentUser.Id)
            {
                return BadRequest<GetReviewByIdResponse>($"{_stringLocalizer["CannotLikeOwnReview"]}");
            }

            // Use current user's ID instead of request.UserId
            var existing = review.LikeDisLikes.FirstOrDefault(l => l.UserId == currentUser.Id);

            if (request.IslikeDisLike == null)
            {
                // Remove like/dislike (undo)
                if (existing != null)
                {
                    review.LikeDisLikes.Remove(existing);
                }
                // If no existing like/dislike, nothing to remove
            }
            else
            {
                // Add or update like/dislike
                if (existing != null)
                {
                    // Update existing
                    existing.IsLike = request.IslikeDisLike.Value;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new
                    var like = new LikeDisLike
                    {
                        ReviewId = request.ReviewId,
                        UserId = currentUser.Id, // Use current user's ID
                        IsLike = request.IslikeDisLike.Value,
                        CreatedAt = DateTime.UtcNow
                    };
                    review.LikeDisLikes.Add(like);
                }
            }

            await unitOfWork.CommitAsync();
            var revMap = review.Adapt<GetReviewByIdResponse>();
            return Success(revMap);
        }
    }
}