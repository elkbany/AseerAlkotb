using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Authors.Requests
{
   public record AddAuthorRequest(string Name,string Bio ,IFormFile? Image);
    
}
