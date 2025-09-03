using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AseerAlkotb.Application.Features.Roles.Requests
{
    public record UpdateAdminAccountRequest
    {
      public  string? FirstName { get; set; }
      public string? LastName { get; set; }
      public string? UserName { get; set; }
      public string? Email {  get; set; }
      public string? PhoneNumber { get; set; }
      public IFormFile? ProfilePictureUrl { get; set; }
      public string? Nationality { get; set; }
      public DateTime? DateOfBirth { get; set; }
      public Gender? Gender { get; set; }
    }
       
    
    
}
