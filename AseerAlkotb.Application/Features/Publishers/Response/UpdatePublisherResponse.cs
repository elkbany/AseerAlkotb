using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Response
{
    public record UpdatePublisherResponse(int Id, string Name, string Description, string LogoUrl, string ContactEmail);

   
}
