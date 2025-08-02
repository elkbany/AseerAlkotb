using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Application.Features.Categories.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class CategoryServices : AppService, ICategoryServices
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddCategoryResponse>> AddCategoryAsync(AddCategoryRequest request)
        {
            await DoValidationAsync<AddCategoryRequestValidator, AddCategoryRequest>(request);
            var category = request.Adapt<category>();
            await unitOfWork.Categories.IInsertAsync(category);
            await unitOfWork.CommitAsync();
            var categoryMap = category.adapt<AddCategoryResponse>();
            return Success(categoryMap);
        }

        public async Task<ApiResponse<DeleteCategoryResponse>>DeleteCategoryAsync(DeleteCategoryRequest request)

        {
            await DoValidationAsync<DeleteCategoryRequestValidator, DeleteCategoryRequest>(request);
            var category = await unitOfWork.Categories.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (category == null)
            {
                return NotFound<DeleteCategoryResponse>("Category not found");
            }

            unitOfWork.Categories.Delete(category);
            await unitOfWork.CommitAsync();
            var categoryMap = category.Adapt<DeleteCategoryResponse>();
            return Success(categoryMap);
        }
        public async Task<ApiResponse<UpdateCategoryResponse>> UpdateCategoryAsync(UpdateCategoryRequest request)
        {
            await DoValidationAsync<UpdateCategoryRequestValidator, UpdateCategoryRequest>(request);
            var category = await unitOfWork.Categories.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (category == null)
            {
                return NotFound<UpdateCategoryResponse>("Category not found");
            }
            category = request.Adapt<Category>();

            unitOfWork.Category.Update(category);
            await unitOfWork.CommitAsync();
            var categoryMap = category.Adapt<UpdateCategoryResponse>();
            return Success(categoryMap);
        }

        public async Task<ApiResponse<GetCategoryByIdResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request)
        {
            await DoValidationAsync<GetCategoryByIdRequestValidator, GetCategoryByIdRequest>(request);
            var category = await unitOfWork.Categories.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (category == null)
            {
                return NotFound<GetCategoryByIdResponse>("Auhtor not found");
            }
            var categoryMap = category.Adapt<GetCategoryByIdResponse>();
            return Success(categoryMap);
        }

        public async Task<ApiResponsePaginated<List<GetAllCategoriesPaginatedResponse>>> GetAllCategoriesPaginatedAsync(GetAllCategoriesPaginatedRequest request)
        {
            await DoValidationAsync<GetAllCategoriesPaginatedRequestValidator, GetAllCategoriesPaginatedRequest>(request);
            var categories = await unitOfWork.Categories
                .GetAllAsync(s => s.Name.Contains(request.Search),
                (request.PageNumber - 1) * request.PageSize, request.PageSize);
            var totalCount = await unitOfWork.Categories.CountAsync((s => s.Name.Contains(request.Search)));
            var authsMap = categories.Adapt<List<GetAllCategoriesPaginatedResponse>>();

            return Success(authsMap, totalCount, request.PageNumber, request.PageSize);

        }

    }
}