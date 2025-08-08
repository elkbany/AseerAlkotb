using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Mapping
{
    public class AuthorMapping : IRegister  
    {
        public void Register(TypeAdapterConfig config)  
        {
            #region Open it to know how to map
            /*TypeAdapterConfig<TSource,TDestination>.NewConfig();
                  --> the source what we convert *from* 
                  --> the destination what we convert *to*
                 we teach it how to map if needed 
                    ( different names)
               note :
               Adding/updating the entity has 2 maps
                --> in Adding it maps :
                    * the request to the entity so we can add it in 
                     the database 
                    * the entity to the response (we created )
                --> in Updating the senario 
                         *at the you must have 6 mapping*
            */
            #endregion

            config.NewConfig<AddAuthorRequest, Author>()
                .Ignore(a => a.Books);

            config.NewConfig<Author, AddAuthorResponse>();
            config.NewConfig<Author, GetAuthorByIdResponse>()
                // For CountryCode , we assume it's an enum and we want to convert it to string
                .Map(dest => dest.CountryCode, src => src.CountryCode.ToString())
                
                .Map(dest => dest.Books, src => src.Books)
                .Ignore(dest=>dest.Rating);
    
            config.NewConfig<Author, GetAllAuthorsPaginatedResponse>()
               .Map(dest => dest.Books, src => src.Books)
               .Ignore(dest=>dest.Rating);





            config.NewConfig<Author, DeleteAuthorResponse>();
            config.NewConfig<UpdateAuthorRequest, Author>();
            config.NewConfig<Author, UpdateAuthorResponse>();
        }
    }
}
