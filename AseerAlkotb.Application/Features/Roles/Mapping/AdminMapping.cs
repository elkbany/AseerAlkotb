using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Account.Requests;
using AseerAlkotb.Application.Features.Account.Responses;
using AseerAlkotb.Application.Features.Roles.Requests;
using AseerAlkotb.Application.Features.Roles.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;

namespace AseerAlkotb.Application.Features.Roles.Mapping
{
    public class AdminMapping:IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateAdminAccountRequest, User>()
                  .Ignore(dest => dest.Id)                    // Managed by Identity
                  .Ignore(dest => dest.ProfilePictureUrl)     // Not in request
                  .Ignore(dest => dest.DateOfBirth)           // Not in request
                  .Ignore(dest => dest.Bio)                   // Not in request
                  .Ignore(dest => dest.IsActive)              // Set manually in service
                  .Ignore(dest => dest.CreatedAt)             // Set manually in service
                  .Ignore(dest => dest.UpdatedAt)             // Set manually in service

                  // Ignore navigation properties (they are relationships, not data to map)
                  .Ignore(dest => dest.Cart)
                  .Ignore(dest => dest.Orders)
                  .Ignore(dest => dest.Reviews)
                  .Ignore(dest => dest.Quotes)
                  .Ignore(dest => dest.Wishlist)
                  .Ignore(dest => dest.LikeDisLikes)
                  .Ignore(dest => dest.Following);



            config.NewConfig<User, CreateAdminAccountResponse>()
               .Map(dest => dest.PhoneNumber, src => src.PhoneNumber ?? string.Empty) 
               .Map(dest => dest.Nationality, src => src.Nationality ?? string.Empty);


            config.NewConfig<User, DeleteAdminAccountResponse>();

            //config.NewConfig<UpdateAdminAccountRequest, User>()
            //    .Ignore(dest => dest.ProfilePictureUrl) // we handle this separately
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.FirstName), dest => dest.FirstName)
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.LastName), dest => dest.LastName)
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.UserName), dest => dest.UserName)
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.Email), dest => dest.Email)
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.PhoneNumber), dest => dest.PhoneNumber)
            //    .IgnoreIf((src, dest) => string.IsNullOrEmpty(src.Nationality), dest => dest.Nationality)
            //    .IgnoreIf((src, dest) => src.DateOfBirth == null, dest => dest.DateOfBirth)
            //    .IgnoreIf((src, dest) => src.Gender == null, dest => dest.Gender);

            config.NewConfig<User,UpdateAdminAccountResponse>();

            config.NewConfig<User, GetAllAdminResponse>();
            config.NewConfig<User, GetAllClientResponse>();





        }
    }
}
