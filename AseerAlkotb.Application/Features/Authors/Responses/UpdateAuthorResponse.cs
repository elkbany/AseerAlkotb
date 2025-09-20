

using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record UpdateAuthorResponse(int Id, string Name,string Name_en, string Bio,string Bio_en, string Image, CountryCode CountryCode);
    
}
