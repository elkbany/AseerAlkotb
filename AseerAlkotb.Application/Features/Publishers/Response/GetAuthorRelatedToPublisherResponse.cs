using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Response
{
    public record GetAuthorRelatedToPublisherResponse(int Id, string Name, string Bio, string? ImageUrl);

}
