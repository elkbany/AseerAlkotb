using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record GetAllSubCategoriesPaginatedRequest(
         int ParentCategoryId,
         int PageNumber = 1,
         int PageSize = 10,
         string Search = ""
     );
}
