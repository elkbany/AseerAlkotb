using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAuthorByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public string CountryCode { get; set; }
        public List<BookCardDto> Books { get; set; }
    }

}
