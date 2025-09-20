using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Requests
{
   public record GetAuthorRelatedToPublisherRequest(int publisherId, int PageNumber = 1, int PageSize = 10);
}
