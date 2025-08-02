using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Authors.Validators;
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

        public async Task<ApiResponse<AddCategoryResponse>>AddCategoryAsync(AddCategoryRequest request)
        {
            await DoValidationAsync<AddCategoryRequestValidator, AddCategoryRequest>(request);
        }
    }
}