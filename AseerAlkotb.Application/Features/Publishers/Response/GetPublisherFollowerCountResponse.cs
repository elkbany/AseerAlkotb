using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Publishers.Response
{
   public class GetPublisherFollowerCountResponse
    {
        public int PublisherId { get; init; }
        public int FollowerCount { get; init; }
    }
}

