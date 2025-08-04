using AseerAlkotb.Application.Features.Books.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAllAuthorsPaginatedResponse(
        int Id,
        string Name,
        string Bio,
        string ImageUrl,
        List<BookCardDto> Books
        );
    
}
