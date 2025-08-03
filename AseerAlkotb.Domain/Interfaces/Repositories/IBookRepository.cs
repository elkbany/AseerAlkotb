using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IBookRepository : IGenericRepository<Book,int>
    {
        public IQueryable<Book> GetQueryable();
    }
}
