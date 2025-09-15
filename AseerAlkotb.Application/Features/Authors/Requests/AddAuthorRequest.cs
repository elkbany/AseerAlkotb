using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;


namespace AseerAlkotb.Application.Features.Authors.Requests
{
   public record AddAuthorRequest(
       string Name, 
       string? Name_en, 
       string Bio, 
       string? Bio_en, 
       IFormFile? Image, 
       CountryCode CountryCode
   );
    
}
