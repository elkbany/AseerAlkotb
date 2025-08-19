using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Account.Responses
{
    public record LoginResponse
    {
        public int Id { get; set; }
        public string Token { get; init; }
        public DateTime Expiration { get; init; }
    }
    
}
