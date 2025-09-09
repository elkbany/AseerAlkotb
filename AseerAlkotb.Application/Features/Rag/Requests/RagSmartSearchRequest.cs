using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Rag.Requests
{
    public record RagSmartSearchRequest(string Query, int TopK = 8);

}
