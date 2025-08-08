using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Requests
{
    public record AddSubCategoryRequest(string Name, string? Description, bool IsActive, int ParentCategoryId);

}
