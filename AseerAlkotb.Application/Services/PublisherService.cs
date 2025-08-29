using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Authors.Validators;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Application.Features.Publishers.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Localization.Resources;
using Mapster;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
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
        public PublisherService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHostEnvironment environment ) : base(serviceProvider, environment)
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
                return NotFound<DeletePublisherResponse>($"{_stringLocalizer["Publisher"]} {_stringLocalizer["NotFound"]}");
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

            var response = publishers
                .Select(p =>
                {
                    var dto = p.Adapt<GetAllPublisherPaginatedResponse>();
                    return dto with
                    {
                        Name = LocalizeEntity("Publisher", dto.Id, "Name", dto.Name),
                        Description = LocalizeEntity("Publisher", dto.Id, "Description", dto.Description)
                    };
                })
                .ToList();

            return Success(response , totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<ApiResponse<GetPublisherByIdResponse>> GetPublisherByIdAsync(GetPublisherByIdRequest request)
        {
            await DoValidationAsync<GetPublisherByIdRequestValidator, GetPublisherByIdRequest>(request);
            var publisher = await unitOfWork.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (publisher == null)
            {
                return NotFound<GetPublisherByIdResponse>($"{_stringLocalizer["Publisher"]} {_stringLocalizer["NotFound"]}");
            }
            var response = publisher.Adapt<GetPublisherByIdResponse>();
            var localized = response with
            {
                Name = LocalizeEntity("Publisher", publisher.Id, "Name", response.Name),
                Description = LocalizeEntity("Publisher", publisher.Id, "Description", response.Description)
            };
            return Success(localized);
        }

        public async Task<ApiResponse<UpdatePublisherResponse>> UpdatePublisherAsync(UpdatePublisherRequest request)
        {
            await DoValidationAsync<UpdatePublisherRequestValidator, UpdatePublisherRequest>(request);

            var publisher = await unitOfWork.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id);
            if (publisher == null)
            {
                return NotFound<UpdatePublisherResponse>($"{_stringLocalizer["Publisher"]} {_stringLocalizer["NotFound"]}");
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<ApiResponse<FollowPublisherResponse>> FollowPublisher(FollowPublisherRequest request)
        {
            await DoValidationAsync<FollowPublisherRequestValidation, FollowPublisherRequest>(request);
            if (await unitOfWork.Publishers.IsFollowingPublisher(request.UserId, request.UserId))
            {
                return BadRequest<FollowPublisherResponse>("You are already following this publisher");
            }
            else
            {
                var userFollow = await unitOfWork.Publishers.FollowPublisher(request.UserId, request.PublisherId);
                await unitOfWork.CommitAsync();
                var response = userFollow.Adapt<FollowPublisherResponse>();
                return Success(response);
            }
        }

        public async Task<ApiResponse<UnFollowPublisherResponse>> UnFollowPublisher(UnFollowPublisherRequest request)
        {
            await DoValidationAsync<UnFollowPublisherRequestValidation, UnFollowPublisherRequest>(request);
            if (await unitOfWork.Publishers.IsFollowingPublisher(request.UserId, request.PublisherId))
            {
                var userFollow = await unitOfWork.Publishers.UnFollowPublisher(request.UserId, request.PublisherId);
                await unitOfWork.CommitAsync();
                var response = userFollow.Adapt<UnFollowPublisherResponse>();
                return Success(response);
            }
            else
            {
                return BadRequest<UnFollowPublisherResponse>("You are not following this Publisher");
            }
        }

        public async Task<ApiResponse<GetPublisherFollowerCountResponse>> GetPublisherFollowerCount(GetPublisherFollowerCountRequest request)
        {
            await DoValidationAsync<GetPublisherFollowerCountRequestValidation, GetPublisherFollowerCountRequest>(request);
            var count = await unitOfWork.Publishers.GetPublisherFollowerCount(request.PublisherId);
            var response = new GetPublisherFollowerCountResponse()
            {
                PublisherId = request.PublisherId,
                FollowerCount = count
            };
            return Success(response);
        }

        public async Task<ApiResponse<List<GetFollowedPublisherResponse>>> GetFollowedPublisher(GetFollowedPublisherRequest request)
        {
            await DoValidationAsync<GetFollowedPublisherRequestValidation, GetFollowedPublisherRequest>(request);
            var Publishers = unitOfWork.Publishers.GetFollowedPublisher(request.UserId).ToList();
            var response = Publishers.Adapt<List<GetFollowedPublisherResponse>>();
            return Success(response);

        }

        public async Task<ApiResponse<List<GetFollowersPublisherResponse>>> GetFollowerPublisher(GetFollowersPublisherRequest request)
        {
            await DoValidationAsync<GetFollowersPublisherRequestValidation, GetFollowersPublisherRequest>(request);
            var Publishers = unitOfWork.Publishers.GetFollowerPublisher(request.publisherId).ToList();
            var response = Publishers.Adapt<List<GetFollowersPublisherResponse>>();
            return Success(response);

        }
    }
}
