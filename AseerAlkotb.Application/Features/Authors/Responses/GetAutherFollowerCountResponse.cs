using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAutherFollowerCountResponse
    {
        public int AuthorId { get; init; }
        public int FollowerCount { get; init; }
    }
}
    

