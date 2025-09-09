using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Rag.Responses
{
    public class ChatSource
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string? CoverImageUrl { get; set; }
        public string? Snippet { get; set; }
    }
}
