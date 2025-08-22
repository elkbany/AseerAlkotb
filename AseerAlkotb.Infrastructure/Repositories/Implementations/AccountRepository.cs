using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        protected readonly ApplicationDbContext dbContext;
        public AccountRepository(ApplicationDbContext _dbContext) 
        {
            dbContext = _dbContext;
        }

        public async Task<User>GetUserWithRelatedData (int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Book)
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Author)
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.LikeDisLikes)

                .Include(u => u.Following)
                    .ThenInclude(f => f.Author)
                .Include(u => u.Following)
                    .ThenInclude(f => f.Publisher)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;

        }
    }
}
