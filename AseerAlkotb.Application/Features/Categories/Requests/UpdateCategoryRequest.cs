using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record UpdateCategoryRequest(int Id, string Name, string? Description, bool IsActive);

}