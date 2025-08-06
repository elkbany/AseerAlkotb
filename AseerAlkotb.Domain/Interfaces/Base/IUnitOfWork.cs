using AseerAlkotb.Domain.Interfaces.Repositories;
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
        public IAuthorRepository Authors { get; }
        public ICategoryRepository Categories { get; }

        public IBookRepository Books { get; }


        public IPublisherRepository Publishers { get;}


        public IReviewRepository Reviews { get; }

        public Task<int> CommitAsync();
    }
}
