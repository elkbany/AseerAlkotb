

namespace AseerAlkotb.Application.Features.Authors.Requests
{
     public record GetAllAuthorsPaginatedRequest(int PageNumber=1,int PageSize=10,string Search="");
   
}
