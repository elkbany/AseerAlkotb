using AseerAlkotb.Application.Features.Books.DTOs;


namespace AseerAlkotb.Application.Features.Authors.Responses
{
    public record GetAuthorByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string ImageUr { get; set; }
        public int Rating { get; set; } // now mutable
        public List<BookCardDto> Books { get; set; }
    }
}
