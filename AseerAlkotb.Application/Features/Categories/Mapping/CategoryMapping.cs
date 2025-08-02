using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Mapping
{
    public class CategoryMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            TypeAdapterConfig<AddCategoryRequest, Category>.NewConfig();
            config.NewConfig<AddCategoryRequest, Category>()
                .Ignore(a => a.SubCategory)
                .Ignore(a => a.ParentCategory);

            config.NewConfig<Category, AddCategoryResponse>();
            config.NewConfig<Category, GetCategoryByIdResponse>();
            config.NewConfig<Category, GetAllCategoriesPaginatedResponse>();
            config.NewConfig<Category, DeleteCategoryResponse>();
            config.NewConfig<UpdateCategoryRequest, Category>();
            config.NewConfig<Category, UpdateCategoryResponse>();
        }
    }
}