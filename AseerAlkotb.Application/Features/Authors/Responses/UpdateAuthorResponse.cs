

using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record UpdateAuthorResponse(int Id, string Name, string Bio, string Image, CountryCode CountryCode);
    
}
