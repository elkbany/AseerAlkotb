using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Books.DTOs;
using AseerAlkotb.Domain.Enums;

namespace AseerAlkotb.Application.Features.Account.Responses
{
    public record GetProfileResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public TimeSpan RegistrationPeriod { get; set; }
        public List<ReviewDto> Reviews { get; set; } = [];
        public List<QuoteDto> Quotes { get; set; } = [];


        public List<UserFollowDto> Following { get; set; } = [];
    }

    public record ReviewDto
    {
       public int Id { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; } 
        public int? BookId { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }

    }

    public record QuoteDto
    {
        public int Id { get; set; }
        public string QouteComment { get; set; }

        //public QuoteFor QuoteFor { get; set; }
    }


    public record UserFollowDto
    {
        public int Id { get; set; }
        public FollowType FollowType { get; set; }
    }



}
