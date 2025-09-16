using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record UpdateCategoryRequest(
        int Id, 
        string Name, 
        string? Name_en, 
        string? Description, 
        string? Description_en, 
        bool IsActive
    );

}