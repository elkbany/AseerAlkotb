using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> IsFollowingPublisher(int userId, int publisherId)
        {
            return await context.UserFollows.AnyAsync(a => a.UserId == userId && a.PublisherId == publisherId);
        }

        public async Task<UserFollow> FollowPublisher(int userId, int publisherId)
        {
            var userFollow = new UserFollow()
            {
                UserId = userId,
                PublisherId = publisherId,
                FollowType = FollowType.Publisher,
            };
            await context.UserFollows.AddAsync(userFollow);
            return userFollow;
        }
        public async Task<UserFollow> UnFollowPublisher(int userId, int publisherId)
        {
            var userFollow = await context.UserFollows.FirstOrDefaultAsync(u => u.UserId == userId && u.PublisherId == publisherId);
            context.UserFollows.Remove(userFollow);
            return userFollow;
        }

        public async Task<int> GetPublisherFollowerCount(int publisherId)
        {
            return await context.UserFollows.CountAsync(f => f.PublisherId == publisherId);
        }

        public IQueryable<Author> GetFollowedPublisher(int userId) //belong user
        {
            return context.UserFollows.Where(u => u.UserId == userId && u.FollowType == FollowType.Publisher)
                .Include(u => u.Author)
                .Select(u => u.Author);
        }


        public IQueryable<User> GetFollowerPublisher(int publisherId)  //belong publisher
        {
            return context.UserFollows.Where(u => u.PublisherId == publisherId)
                .Include(u => u.User)
                .Select(u => u.User);
        }
        public IQueryable<Author> GetAuthorsRelatededToPublisher(int publisherId)
        {
            return context.Publishers
                .Where(p => p.Id == publisherId)
                .SelectMany(p => p.Books)
                .Select(b => b.Author)
                .Distinct();         // Remove duplicate authors
        }
    }
}
