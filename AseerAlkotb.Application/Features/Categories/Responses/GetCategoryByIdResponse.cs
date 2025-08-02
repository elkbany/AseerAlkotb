using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Responses
{
    public record GetCategoryByIdResponse(string Name, string? Description, bool IsActive);

}