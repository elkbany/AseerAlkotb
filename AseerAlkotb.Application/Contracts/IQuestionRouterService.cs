using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Application.Features.Rag.Models;

namespace AseerAlkotb.Application.Contracts
{
    public interface IQuestionRouterService
    {
        Task<RouteResult> RouteAsync(string question);
    }
}
