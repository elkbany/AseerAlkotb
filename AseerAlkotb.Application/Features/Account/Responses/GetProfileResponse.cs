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
        public TimeSpan RegistrationPeriod { get; set; }
        public List<ReviewDto> Reviews { get; set; } = [];
        public List<UserFollowDto> Following { get; set; } = [];
    }

    public record ReviewDto
    {
       public int Id { get; set; }
        public ReviewFor ReviewFor { get; set; }
    }
    public record UserFollowDto
    {
        public int Id { get; set; }
        public FollowType FollowType { get; set; }
    }

}
