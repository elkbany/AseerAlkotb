

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
    public class AuthorServices : AppService
    {
        private readonly IUnitOfWork unitOfWork;

        public AuthorServices(IUnitOfWork unitOfWork , IServiceProvider serviceProvider,IHostEnvironment environment) : base(serviceProvider,environment) 
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<AddAuthorResponse>> AddAuthorAsync(AddAuthorRequest request)
        {
            await DoValidationAsync<AddAuthorRequestValidator,AddAuthorRequest>(request);
            var author = request.Adapt<Author>();
            await unitOfWork.Authors.InsertAsync(author);
            await unitOfWork.CommitAsync();

            var authMap = author.Adapt<AddAuthorResponse>();
            return Success(authMap);
        }
    }
}
