using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.ResponseHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IPublisherServices
    {
        public Task<ApiResponsePaginated<List<GetAllPublisherPaginatedResponse>>> GetAllPublishersPaginatedAsync(GetAllPublishersPaginatedRequest request);
        public Task<ApiResponse<GetPublisherByIdResponse>> GetPublisherByIdAsync(GetPublisherByIdRequest request);

        public Task<ApiResponse<UpdatePublisherResponse>> UpdatePublisherAsync(UpdatePublisherRequest request);

        public Task<ApiResponse<DeletePublisherResponse>> DeletePublisherAsync(DeletePublisherRequest request);

        public Task<ApiResponse<AddPublisherResponse>> AddPublisherAsync(AddPublisherRequest request);

    }
}
