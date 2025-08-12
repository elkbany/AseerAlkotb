using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.ResponseHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IAuthorServices
    {
        Task<ApiResponsePaginated<List<GetAllAuthorsPaginatedResponse>>> GetAllAuthorsPaginatedAsync(GetAllAuthorsPaginatedRequest request);
        Task<ApiResponse<GetAuthorByIdResponse>> GetAuthorByIdAsync(GetAuthorByIdRequest request);
        Task<ApiResponse<UpdateAuthorResponse>> UpdateAuthorAsync(UpdateAuthorRequest request);
        Task<ApiResponse<DeleteAuthorResponse>> DeleteAuthorAsync(DeleteAuthorRequest request);
        Task<ApiResponse<AddAuthorResponse>> AddAuthorAsync(AddAuthorRequest request);
        /////////////////////////////////////////////////////////////////////////////////////
        Task<ApiResponse<FollowAutherResponse>> FollowAuther(FollowAutherRequest request);

        Task<ApiResponse<UnFollowAuthorResponse>> UnFollowAuthor(UnFollowAuthorRequest request);


        Task<ApiResponse<GetAutherFollowerCountResponse>> GetAutherFollowerCount(GetAutherFollowerCountRequest request);


        Task<ApiResponse<List<GetFollowedAuthorResponse>>> GetFollowedAuther(GetFollowedAuthorRequest request);


        Task<ApiResponse<List<GetFollowersAuthorResponse>>> GetFollowerAuther(GetFollowersAuthorRequest request);
        
    }
}
