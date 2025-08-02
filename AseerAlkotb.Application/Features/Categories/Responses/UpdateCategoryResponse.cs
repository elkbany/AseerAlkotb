namespace AseerAlkotb.Application.Features.Categories.Responses
{
    public record UpdateCategoryResponse(int Id, string Name, string? Description, bool IsActive);
}