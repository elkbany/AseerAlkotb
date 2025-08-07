using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Responses
{
    public record AddSubCategoryResponse(int Id, string Name, string? Description, bool IsActive, int ParentCategoryId);
}
