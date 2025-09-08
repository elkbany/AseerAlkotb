using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Rag.Requests
{
    public record RagAskRequest(string Question, string? Language = null, string? Category = null, int Limit = 5);

}
