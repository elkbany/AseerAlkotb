namespace AseerAlkotb.Application.Features.Categories.Responses
{
    public record UpdateCategoryResponse(int Id, string Name,string Name_en,string? Description_en,string? Description, bool IsActive);
}