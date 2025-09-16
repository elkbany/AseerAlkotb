using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Requests
{
    public record AddPublisherRequest(
        string Name, 
        string? Name_en, 
        string Description, 
        string? Description_en, 
        IFormFile? LogoUrl, 
        string ContactEmail
    );

}
