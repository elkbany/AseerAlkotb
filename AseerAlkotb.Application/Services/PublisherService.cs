using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.Features.Publishers.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
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
    public class PublisherService : AppService , IPublisherServices
    {
        private readonly IUnitOfWork unitOfWork;
        public PublisherService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment) : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AddPublisherResponse>> AddPublisherAsync(AddPublisherRequest request)
        {
            await DoValidationAsync<AddPublisherRequestValidator, AddPublisherRequest>(request);
            var publisher = request.Adapt<Publisher>();
            if (request.LogoUrl != null)
            {
                publisher.LogoUrl = await UploadImageAsync(request.LogoUrl, "Publishers");
            }
            await unitOfWork.Publishers.InsertAsync(publisher);
            await unitOfWork.CommitAsync();

            var response = publisher.Adapt<AddPublisherResponse>();

            return Success(response);
        }

        public async Task<ApiResponse<DeletePublisherResponse>> DeletePublisherAsync(DeletePublisherRequest request)
        {
            await DoValidationAsync<DeletePublisherRequestValidator, DeletePublisherRequest>(request);
            var publisher = await unitOfWork.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (publisher == null)
            {
                return NotFound<DeletePublisherResponse>("Publisher not found");
            }
            if (publisher.LogoUrl != null)
            {
                await DeleteImageAsync(publisher.LogoUrl);
            }
            unitOfWork.Publishers.Delete(publisher);
            await unitOfWork.CommitAsync();

            var response = publisher.Adapt<DeletePublisherResponse>();

            return Success(response);
        }

        public async Task<ApiResponsePaginated<List<GetAllPublisherPaginatedResponse>>> GetAllPublishersPaginatedAsync(GetAllPublishersPaginatedRequest request)
        {
            await DoValidationAsync<GetAllPublishersPaginatedRequestValidator, GetAllPublishersPaginatedRequest>(request);
            var publishers = await unitOfWork.Publishers
                .GetAllAsync(search => search.Name.Contains(request.Search) , 
                (request.PageNumber - 1) * request.PageSize, request.PageSize);

            var totalCount = await unitOfWork.Publishers.CountAsync((search => search.Name.Contains(request.Search)));

            var response = publishers.Adapt<List<GetAllPublisherPaginatedResponse>>();

            return Success(response , totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<ApiResponse<GetPublisherByIdResponse>> GetPublisherByIdAsync(GetPublisherByIdRequest request)
        {
            await DoValidationAsync<GetPublisherByIdRequestValidator, GetPublisherByIdRequest>(request);
            var publisher = await unitOfWork.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (publisher == null)
            {
                return NotFound<GetPublisherByIdResponse>("Publisher not found");
            }
            var response = publisher.Adapt<GetPublisherByIdResponse>();
            return Success(response);
        }

        public async Task<ApiResponse<UpdatePublisherResponse>> UpdatePublisherAsync(UpdatePublisherRequest request)
        {
            await DoValidationAsync<UpdatePublisherRequestValidator, UpdatePublisherRequest>(request);

            var publisher = await unitOfWork.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (publisher == null)
            {
                return NotFound<UpdatePublisherResponse>("Publisher not found");
            }
            request.Adapt(publisher);
            if (request.LogoUrl != null)
            {
                publisher.LogoUrl = await UpdateImageAsync(request.LogoUrl,publisher.LogoUrl, "Publishers");
            }
            unitOfWork.Publishers.Update(publisher);
            await unitOfWork.CommitAsync();
            var response = publisher.Adapt<UpdatePublisherResponse>();

            return Success(response);
        }
    }
}
