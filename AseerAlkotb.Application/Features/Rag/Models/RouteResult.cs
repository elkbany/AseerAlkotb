using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Rag.Models
{
    public sealed class RouteResult
    {
        public string intent { get; set; } = "general_recs";
        public RouteEntities entities { get; set; } = new();
        public string language { get; set; } = "ar";
        public double confidence { get; set; }
    }
}
