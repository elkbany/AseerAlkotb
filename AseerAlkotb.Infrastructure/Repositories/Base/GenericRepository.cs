

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Infrastructure.Repositories.Base
{
    public class GenericRepository<TEntity, Key> : IGenericRepository<TEntity, Key>
        where Key : struct
        where TEntity : Entity<Key>
    {
        #region Variables
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> _table;
        #endregion

        #region CTOR
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _table = dbContext.Set<TEntity>();
        }
        #endregion

        #region Async Functions

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetWhere(criteria, includes);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table, includes);
            return await query.ToListAsync(cancellationToken);
        }

        // 🔥 Fixed: Correct pagination implementation
        public async Task<List<TEntity>> GetAllAsync(int skip, int take,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table, includes);
            return await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? criteria,
            int? skip, int? take,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table;

            if (criteria != null)
                query = query.Where(criteria);

            query = Includes(query, includes);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table;

            if (criteria != null)
                query = query.Where(criteria);

            query = Includes(query, includes);

            return await query.ToListAsync(cancellationToken);
        }

        // 🆕 Added: NoTracking versions for better performance
        public async Task<List<TEntity>> GetAllAsNoTrackingAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table.AsNoTracking();

            if (criteria != null)
                query = query.Where(criteria);

            query = Includes(query, includes);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(Key id, CancellationToken cancellationToken = default)
        {
            return await _table.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(Key id, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table, includes);
            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        // 🆕 Added: NoTracking version for read-only operations
        public async Task<TEntity?> GetByIdAsNoTrackingAsync(Key id, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table.AsNoTracking(), includes);
            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        //public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> criteria,
        //    CancellationToken cancellationToken = default,
        //    params Expression<Func<TEntity, object>>[] includes)
        //{
        //    return await GetWhere(criteria, includes).FirstOrDefaultAsync(cancellationToken);
        //}

        public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _table.AddAsync(entity, cancellationToken);
        }

        //public async Task InsertListAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        //{
        //    await _table.AddRangeAsync(entities, cancellationToken);
        //}

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default)
        {
            return criteria == null ?
                await _table.AnyAsync(cancellationToken) :
                await _table.AnyAsync(criteria, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default)
        {
            return criteria == null ?
                await _table.CountAsync(cancellationToken) :
                await _table.CountAsync(criteria, cancellationToken);
        }

        // 🆕 Added: Bulk operations for better performance
        //public async Task<int> BulkDeleteAsync(Expression<Func<TEntity, bool>> criteria,
        //    CancellationToken cancellationToken = default)
        //{
        //    return await _table.Where(criteria).ExecuteDeleteAsync(cancellationToken);
        //}

        //public async Task<int> BulkUpdateAsync(Expression<Func<TEntity, bool>> criteria,
        //    Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
        //    CancellationToken cancellationToken = default)
        //{
        //    return await _table.Where(criteria).ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        //}

        #endregion

        #region Sync Functions (Keep for compatibility, but prefer async)

        public TEntity? GetById(Key id)
        {
            return _table.SingleOrDefault(e => e.Id.Equals(id));
        }

        public List<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table, includes);
            return query.ToList();
        }

        //public TEntity? GetFirst(Expression<Func<TEntity, bool>> criteria,
        //    params Expression<Func<TEntity, object>>[] includes)
        //{
        //    return GetWhere(criteria, includes).FirstOrDefault();
        //}

        public void Insert(TEntity entity)
        {
            _table.Add(entity);
        }

        //public void InsertList(IEnumerable<TEntity> entities)
        //{
        //    _table.AddRange(entities);
        //}

        public void Update(TEntity entity)
        {
            _table.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _table.Remove(entity);
        }

        public void DeleteList(IEnumerable<TEntity> entities) // 🔥 Fixed: Changed from IQueryable to IEnumerable
        {
            _table.RemoveRange(entities);
        }

        #endregion

        #region Enhanced Helper Methods

        private IQueryable<TEntity> Includes(IQueryable<TEntity> query,
            params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return query;
        }

        private IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> criteria,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = Includes(_table, includes);
            return query.Where(criteria);
        }

        public async Task<TEntity?> FindAsync(CancellationToken cancellationToken = default,
            params object[] id)
        {
            return await _table.FindAsync(id, cancellationToken);
        }

        // 🆕 Added: Queryable access for complex scenarios
        //public IQueryable<TEntity> Query()
        //{
        //    return _table.AsQueryable();
        //}

        //public IQueryable<TEntity> QueryAsNoTracking()
        //{
        //    return _table.AsNoTracking();
        //}

        // 🆕 Added: Projection support for better performance
        public async Task<List<TResult>> Select<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? criteria = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _table.AsNoTracking();

            if (criteria != null)
                query = query.Where(criteria);

            return await query.Select(selector).ToListAsync(cancellationToken);
        }

        // 🆕 Added: Ordered queries support
        //public async Task<List<TEntity>> GetAllOrderedAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy,
        //    bool descending = false,
        //    Expression<Func<TEntity, bool>>? criteria = null,
        //    CancellationToken cancellationToken = default,
        //    params Expression<Func<TEntity, object>>[] includes)
        //{
        //    IQueryable<TEntity> query = _table;

        //    if (criteria != null)
        //        query = query.Where(criteria);

        //    query = Includes(query, includes);

        //    query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        //    return await query.ToListAsync(cancellationToken);
        //}

        // 🆕 Added: Pagination support
        //public async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(int skip, int take,
        //    Expression<Func<TEntity, bool>>? criteria = null,
        //    CancellationToken cancellationToken = default,
        //    params Expression<Func<TEntity, object>>[] includes)
        //{
        //    IQueryable<TEntity> query = _table;

        //    if (criteria != null)
        //        query = query.Where(criteria);

        //    query = Includes(query, includes);

        //    var totalCount = await query.CountAsync(cancellationToken);
        //    var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

        //    return (items, totalCount);
        //}

        #endregion

        #region IDisposable Support (Optional)

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // DbContext is usually managed by DI container
                    // Don't dispose it here unless you're sure
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
