using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;

namespace AseerAlkotb.Application.Features.Account.Mapping
{
    public class AccountMapping :IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            TypeAdapterConfig<RegisterRequest, User>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.IsActive, _ => true)
                .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, _ => DateTime.UtcNow)
                .Ignore(dest => dest.Id)
                .Ignore("ConfirmPassword")
                .Ignore("Password")
                .Ignore(dest => dest.Cart)      // Navigation properties 
                .Ignore(dest => dest.Orders)
                .Ignore(dest => dest.Reviews)
                .Ignore(dest => dest.Wishlist)
                .Ignore(dest => dest.LikeDisLikes)
                .Ignore(dest => dest.Following);
            //.IgnoreNonMapped(true);



            TypeAdapterConfig<User, RegisterResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.Id);
        }
    }
}
