using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class PublisherRepository : GenericRepository<Publisher, int> , IPublisherRepository
    {
        private readonly ApplicationDbContext context;
        public PublisherRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}
