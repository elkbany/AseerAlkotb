using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Authors.Responses;
using AseerAlkotb.Application.Features.Books.Mapping;
using AseerAlkotb.Application.Features.Books.Validators;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;


namespace AseerAlkotb.Application.Services
{
    public class BookServices : AppService, IBookServices
    {
        private readonly IUnitOfWork _uniteOfWork;

        public BookServices(IUnitOfWork uniteOfWork, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment): base(serviceProvider, hostEnvironment) 
        {
            _uniteOfWork = uniteOfWork;
        }


        public async Task<ApiResponse<AddBookResponse>> AddBookAsync(AddBookRequest request)
        {
            await DoValidationAsync<AddBookRequestValidator, AddBookRequest>(request);
            
            var book = request.Adapt<Book>();
            if (request.CoverImageUrl != null) 
            {
                book.CoverImageUrl = await UploadImageAsync(request.CoverImageUrl, "Books");
            }

            await _uniteOfWork.Books.InsertAsync(book);
            await _uniteOfWork.CommitAsync();

            var bookMap = book.Adapt<AddBookResponse>();
            return Success(bookMap);
        }
    }
}
