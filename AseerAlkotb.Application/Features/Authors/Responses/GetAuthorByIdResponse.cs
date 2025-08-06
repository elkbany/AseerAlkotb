using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAuthorByIdResponse(int Id, 
        string Name,
        string Bio, 
        string ImageUrl,
        CountryCode CountryCode,
        List<BookCardDto> Books);
   
}
