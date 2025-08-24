using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Publishers.Requests;
using AseerAlkotb.Application.Features.Publishers.Response;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Mapping
{
    public class PublisherMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // FROM Request TO Entity
            TypeAdapterConfig<AddPublisherRequest, Publisher>.NewConfig();
            config.NewConfig<AddPublisherRequest, Publisher>()
                .Ignore(dest => dest.Books);

            config.NewConfig<UpdatePublisherRequest , Publisher>();

            // FROM Entity TO Response
            config.NewConfig<Publisher, AddPublisherResponse>();
            config.NewConfig<Publisher, GetPublisherByIdResponse>();
            config.NewConfig<Publisher, UpdatePublisherResponse>();
            config.NewConfig<Publisher, DeletePublisherResponse>();
            config.NewConfig<Publisher, GetAllPublisherPaginatedResponse>();

            ////////////////////////////////////////Following///////////////////////////////////////////
            config.NewConfig<UserFollow, FollowPublisherResponse>();
            config.NewConfig<UserFollow, UnFollowPublisherResponse>();

            TypeAdapterConfig<Publisher, GetFollowedPublisherResponse>
            .NewConfig()
            .Map(dest => dest.PublisherId, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.LogoUrl, src => src.LogoUrl);

            TypeAdapterConfig<User, GetFollowersPublisherResponse> // what need return
            .NewConfig()
            .Map(dest => dest.UserId, src => src.Id);

        }
    }
}
