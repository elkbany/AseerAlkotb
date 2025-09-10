using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Features.Books.DTOs
{
    public record BookDTO
    (
        string Title,
        decimal Price,
        int Quantity,
        int Id,
        string ImageUrl

    );
}
