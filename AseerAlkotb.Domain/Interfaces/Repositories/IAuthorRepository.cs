

using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IAuthorRepository : IGenericRepository<Author,int>
    {
        Task<bool> IsFollowingAuther(int userId, int authorId);

        Task<UserFollow> FollowAuther(int userId, int authorId);

        Task<UserFollow> UnFollowAuther(int userId, int authorId);

        Task<int> GetAuthorFollowerCount(int autherId);

        IQueryable<Author> GetFollowedAuther(int userId);
        IQueryable<User> GetFollowerAuther(int autherId);
    }
}
