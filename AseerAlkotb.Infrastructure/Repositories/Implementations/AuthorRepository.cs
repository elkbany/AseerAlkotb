using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class AuthorRepository : GenericRepository<Author, int>, IAuthorRepository
    {
        private readonly ApplicationDbContext context;

        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> IsFollowingAuther(int userId, int authorId)
        {
            return await context.UserFollows.AnyAsync(a=>a.UserId==userId && a.AuthorId==authorId);
        }

        public async Task<UserFollow> FollowAuther(int userId, int authorId)
        {
            var userFollow = new UserFollow()
            {
                UserId = userId,
                AuthorId = authorId,
                FollowType = FollowType.Author,
            };
            await context.UserFollows.AddAsync(userFollow);
            return userFollow;
        }
        public async Task<UserFollow> UnFollowAuther(int userId, int authorId)
        {
           var userFollow = await context.UserFollows.FirstOrDefaultAsync(u=>u.UserId==userId && u.AuthorId==authorId);
            context.UserFollows.Remove(userFollow);
            return userFollow;
        }

        public async Task<int>GetAuthorFollowerCount(int autherId)
        {
            return await context.UserFollows.CountAsync(f=>f.AuthorId==autherId);
        }

        public  IQueryable<Author> GetFollowedAuther(int userId) //belong user
        {
            return  context.UserFollows.Where(u => u.UserId == userId && u.FollowType == FollowType.Author)
                .Include(u => u.Author)
                .Select(u => u.Author);
        }


        public IQueryable<User> GetFollowerAuther(int autherId)  //belong Author
        {
            return context.UserFollows.Where(u => u.AuthorId == autherId)
                .Include(u => u.User)
                .Select(u => u.User);
        }
    }
}

