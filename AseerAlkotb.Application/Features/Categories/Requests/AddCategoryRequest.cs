using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record AddCategoryRequest(string Name,string? Description,bool IsActive);

}