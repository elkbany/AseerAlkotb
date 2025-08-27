using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Application.Features.Categories.Responses;
using AseerAlkotb.Domain.Entites.Models;
using Mapster;

namespace AseerAlkotb.Application.Features.Categories.Mapping
{
    public class CategoryMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Basic mappings
            config.NewConfig<AddCategoryRequest, Category>()
                .Ignore(dest => dest.SubCategory)
                .Ignore(dest => dest.ParentCategory)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<UpdateCategoryRequest, Category>()
                .Ignore(dest => dest.SubCategory)
                .Ignore(dest => dest.ParentCategory)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<AddSubCategoryRequest, Category>()
                .Ignore(dest => dest.SubCategory)
                .Ignore(dest => dest.ParentCategory)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            // Response mappings
            config.NewConfig<Category, AddCategoryResponse>();
            config.NewConfig<Category, UpdateCategoryResponse>();
            config.NewConfig<Category, DeleteCategoryResponse>();
            config.NewConfig<Category, GetCategoryByIdResponse>();
            config.NewConfig<Category, AddSubCategoryResponse>();
            config.NewConfig<Category, DeleteSubCategoryResponse>();

            // Complex mappings for Dashboard - simplified
            config.NewConfig<Category, GetAllCategoriesPaginatedResponse>()
                .Map(dest => dest.ParentCategoryId, src => src.ParentCategoryId)
                .Map(dest => dest.SubCategoryCount, src => src.SubCategory != null ? src.SubCategory.Count : 0)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);

            config.NewConfig<Category, GetAllSubCategoriesPaginatedResponse>()
                .Map(dest => dest.ParentCategoryId, src => src.ParentCategoryId ?? 0)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);
        }
    }
}