namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record GetAllCategoriesPaginatedRequest(int PageNumber = 1, int PageSize = 10, string Search = "");

}