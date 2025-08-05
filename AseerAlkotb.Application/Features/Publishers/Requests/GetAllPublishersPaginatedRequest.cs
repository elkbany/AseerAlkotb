using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Requests
{
    public record GetAllPublishersPaginatedRequest(int PageNumber = 1, int PageSize = 10, string Search = "");


}
