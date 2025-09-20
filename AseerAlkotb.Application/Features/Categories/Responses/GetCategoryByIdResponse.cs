using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Categories.Responses
{
    public record GetCategoryByIdResponse(
        int Id, 
        string Name, 
        string Name_en,
        string? Description, 
        string? Description_en,
        bool IsActive,
        DateTime CreatedAt = default,
        DateTime UpdatedAt = default
    );
}