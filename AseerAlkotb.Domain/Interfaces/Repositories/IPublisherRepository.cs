using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IPublisherRepository : IGenericRepository<Publisher , int>
    {
        Task<bool> IsFollowingPublisher(int userId, int publisherId);
       
        Task<UserFollow> FollowPublisher(int userId, int publisherId);


        Task<UserFollow> UnFollowPublisher(int userId, int publisherId);


        Task<int> GetPublisherFollowerCount(int publisherId);


        IQueryable<Author> GetFollowedPublisher(int userId);

        IQueryable<User> GetFollowerPublisher(int publisherId);
        IQueryable<Author> GetAuthorsRelatededToPublisher(int publisherId);

    }
}
