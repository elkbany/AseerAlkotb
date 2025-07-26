using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Domain.Interfaces.Base
{
    public interface IGenericRepository<TEntity, Key> : IDisposable
         where Key : struct
         where TEntity : class
    {
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsync(int skip, int take,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? criteria,
            int? skip, int? take,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<List<TEntity>> GetAllAsNoTrackingAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsync(Key id, CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdAsync(Key id, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsNoTrackingAsync(Key id, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);



        Task<TEntity?> FindAsync(CancellationToken cancellationToken = default,
            params object[] id);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default);

        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);


        void Update(TEntity entity);

        void Delete(TEntity entity);





        TEntity? GetById(Key id);

        List<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);


        void Insert(TEntity entity);


        Task<List<TResult>> Select<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default);

        //Task<List<TEntity>> GetAllOrderedAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy,
        //    bool descending = false,
        //    Expression<Func<TEntity, bool>>? criteria = null,
        //    CancellationToken cancellationToken = default,
        //    params Expression<Func<TEntity, object>>[] includes);

        //Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(int skip, int take,
        //    Expression<Func<TEntity, bool>>? criteria = null,
        //    CancellationToken cancellationToken = default,
        //    params Expression<Func<TEntity, object>>[] includes);
    }
}
