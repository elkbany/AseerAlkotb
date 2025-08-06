using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            // EntityRepository = new  EntityRepository(dbcontext);
            Authors = new AuthorRepository(dbContext);
            Categories = new CategoryRepository(dbContext);

            Books = new BookRepository(dbContext);

            Carts = new CartRepository(dbContext);
            

        }
        //public IEntityrepository EntityRepository {get; private set;}
        public IAuthorRepository Authors { get; private set; }

        public ICategoryRepository Categories { get; private set; }

        public IBookRepository Books { get; private set; }
        public ICartRepository Carts { get; private set; }

        public async Task<int> CommitAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}

