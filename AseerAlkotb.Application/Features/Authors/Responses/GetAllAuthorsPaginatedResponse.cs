using AseerAlkotb.Application.Features.Books.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAllAuthorsPaginatedResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public int Rating { get; set; } // now mutable
        public List<BookCardDto> Books { get; set; }
    }
}
