using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Contracts
{
    public interface IRerankerService
    {
        /// <summary>Re-rank generic items by relevance to query. Returns same items re-ordered.</summary>
        Task<List<T>> RerankAsync<T>(string query, List<T> items, Func<T, string> textSelector, int topK);
    }
}
