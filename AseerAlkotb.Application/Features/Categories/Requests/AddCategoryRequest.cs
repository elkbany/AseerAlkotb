using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record AddCategoryRequest(
        string Name, 
        string? Name_en, 
        string? Description, 
        string? Description_en, 
        bool IsActive
    );

}