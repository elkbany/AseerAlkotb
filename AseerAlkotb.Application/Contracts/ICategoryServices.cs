using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface ICategoryServices
    {
        public Task<ApiResponsePaginated<List<GetAllCategoriesPaginatedResponse>>> GetAllCategoriesPaginatedAsync(GetAllCategoriesPaginatedRequest request);
        public Task<ApiResponse<GetCategoryByIdResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request);
        public Task<ApiResponse<UpdateCategoryResponse>> UpdateCategoryAsync(UpdateCategoryRequest request);
        public Task<ApiResponse<DeleteCategoryResponse>> DeleteCategoryAsync(DeleteCategoryRequest request);
        public Task<ApiResponse<AddCategoryResponse>> AddCategoryAsync(AddCategoryRequest request);
        public Task<ApiResponse<AddSubCategoryResponse>> AddSubCategoryAsync(AddSubCategoryRequest request);
        public Task<ApiResponse<DeleteSubCategoryResponse>> DeleteSubCategoryAsync(DeleteSubCategoryRequest request);
        public Task<ApiResponsePaginated<List<GetAllSubCategoriesPaginatedResponse>>> GetAllSubCategoriesPaginatedAsync(GetAllSubCategoriesPaginatedRequest request);
    }
}
