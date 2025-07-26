using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Base
{
    public interface IUnitOfWork
    {
        //public IEntityRepository EntityRepository {get;}
        public Task<int> CommitAsync();
    }
}
