using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Application.Features.Categories.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Resources;
using Mapster;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class CategoryServices : AppService, ICategoryServices
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryServices(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) : base(serviceProvider, environment )
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddCategoryResponse>> AddCategoryAsync(AddCategoryRequest request)
        {
            await DoValidationAsync<AddCategoryRequestValidator, AddCategoryRequest>(request);
            var category = request.Adapt<Category>();
            await unitOfWork.Categories.InsertAsync(category);
            await unitOfWork.CommitAsync();
            var categoryMap = category.Adapt<AddCategoryResponse>();
            return Success(categoryMap);
        }

        public async Task<ApiResponse<DeleteCategoryResponse>>DeleteCategoryAsync(DeleteCategoryRequest request)

        {
            await DoValidationAsync<DeleteCategoryRequestValidator, DeleteCategoryRequest>(request);
            var category = await unitOfWork.Categories.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (category == null)
            {
                return NotFound<DeleteCategoryResponse>($"{_stringLocalizer["Category"]} {_stringLocalizer["NotFound"]}");
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
                return NotFound<UpdateCategoryResponse>($"{_stringLocalizer["Category"]} {_stringLocalizer["NotFound"]}");
            }
            request.Adapt(category);
            unitOfWork.Categories.Update(category);
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
                return NotFound<GetCategoryByIdResponse>($"{_stringLocalizer["Category"]} {_stringLocalizer["NotFound"]}");
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

        public async Task<ApiResponse<AddSubCategoryResponse>> AddSubCategoryAsync(AddSubCategoryRequest request)
        {
            await DoValidationAsync<AddSubCategoryRequestValidator, AddSubCategoryRequest>(request);

            var parentCategory = await unitOfWork.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId);
            if (parentCategory == null)
            {
                return NotFound<AddSubCategoryResponse>($"{_stringLocalizer["ParentCategory"]} {_stringLocalizer["NotFound"]}");
            }

            var subCategory = request.Adapt<Category>();
            await unitOfWork.Categories.InsertAsync(subCategory);
            await unitOfWork.CommitAsync();

            var response = subCategory.Adapt<AddSubCategoryResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<DeleteSubCategoryResponse>> DeleteSubCategoryAsync(DeleteSubCategoryRequest request)
        {
            await DoValidationAsync<DeleteSubCategoryRequestValidator, DeleteSubCategoryRequest>(request);

            var subCategory = await unitOfWork.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && c.ParentCategoryId == request.ParentCategoryId);
            if (subCategory == null)
            {
                return NotFound<DeleteSubCategoryResponse>($"{_stringLocalizer["SubCategory"]} {_stringLocalizer["NotFound"]}");
            }

            unitOfWork.Categories.Delete(subCategory);
            await unitOfWork.CommitAsync();

            var response = subCategory.Adapt<DeleteSubCategoryResponse>();
            return Success(response);
        }

        public async Task<ApiResponsePaginated<List<GetAllSubCategoriesPaginatedResponse>>> GetAllSubCategoriesPaginatedAsync(GetAllSubCategoriesPaginatedRequest request)
        {
            await DoValidationAsync<GetAllSubCategoriesPaginatedRequestValidator, GetAllSubCategoriesPaginatedRequest>(request);

            var query = await unitOfWork.Categories.GetAllAsync(
                c => c.ParentCategoryId == request.ParentCategoryId && c.Name.Contains(request.Search),
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize
            );

            var totalCount = await unitOfWork.Categories.CountAsync(c => c.ParentCategoryId == request.ParentCategoryId && c.Name.Contains(request.Search));

            var result = query.Adapt<List<GetAllSubCategoriesPaginatedResponse>>();

            return Success(result, totalCount, request.PageNumber, request.PageSize);
        }


    }
}