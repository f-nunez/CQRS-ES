using System.Linq.Expressions;
using CQRSES.Query.Domain.Common;

namespace CQRSES.Query.Application.Common;

public interface IRepository<T> where T : Entity
{
    long Count(Expression<Func<T, bool>>? filter = null);
    Task<long> CountAsync(Expression<Func<T, bool>>? filter = null);
    void Delete(T entity);
    Task DeleteAsync(T entity);
    void DeleteById(string id);
    Task DeleteByIdAsync(string id);
    void DeleteRange(IEnumerable<T> entities);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    bool Exists(Expression<Func<T, bool>> filter);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
    T? GetFirstOrDefault(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    T GetById(string id);
    Task<T> GetByIdAsync(string id);
    IEnumerable<T> GetList(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null);
    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null);
    void Insert(T entity);
    Task InsertAsync(T entity);
    void InsertRange(IEnumerable<T> entities);
    Task InsertRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void UpdateRange(IEnumerable<T> entities);
    Task UpdateRangeAsync(IEnumerable<T> entities);
}