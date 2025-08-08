using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Authors.Requests
{
    public record UpdateAuthorRequest(int Id, string Name, string Bio, IFormFile? Image, CountryCode CountryCode);
  
}
 