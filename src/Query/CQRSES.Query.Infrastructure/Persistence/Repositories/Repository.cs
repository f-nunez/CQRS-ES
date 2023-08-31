using System.Linq.Expressions;
using CQRSES.Query.Application.Common;
using CQRSES.Query.Domain.Common;
using MongoDB.Driver;

namespace CQRSES.Query.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _mongoCollection;

    public Repository(IMongoDbContext mongoDbContext)
    {
        _mongoCollection = mongoDbContext.GetCollection<T>();
    }

    public long Count(Expression<Func<T, bool>>? filter = null)
    {
        var filterDefinition = Builders<T>.Filter.Where(filter);

        return _mongoCollection.CountDocuments(filterDefinition);
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? filter = null)
    {
        var filterDefinition = Builders<T>.Filter.Where(filter);

        return await _mongoCollection.CountDocumentsAsync(filterDefinition);
    }

    public void Delete(T entity)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == entity.Id);

        _mongoCollection.DeleteOne(filterDefinition);
    }

    public async Task DeleteAsync(T entity)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == entity.Id);

        await _mongoCollection.DeleteOneAsync(filterDefinition);
    }

    public void DeleteById(string id)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == id);

        _mongoCollection.DeleteOne(filterDefinition);
    }

    public async Task DeleteByIdAsync(string id)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == id);

        await _mongoCollection.DeleteOneAsync(filterDefinition);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => entities.Any(y => y.Id == x.Id));

        _mongoCollection.DeleteMany(filterDefinition);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => entities.Any(y => y.Id == x.Id));

        await _mongoCollection.DeleteManyAsync(filterDefinition);
    }

    public bool Exists(Expression<Func<T, bool>> filter)
    {
        var filterDefinition = Builders<T>.Filter.Where(filter);

        long count = _mongoCollection.CountDocuments(filterDefinition);

        return count > 0;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        var filterDefinition = Builders<T>.Filter.Where(filter);

        long count = await _mongoCollection.CountDocumentsAsync(filterDefinition);

        return count > 0;
    }

    public T GetById(string id)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == id);

        return _mongoCollection.Find(filterDefinition).FirstOrDefault();
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var filterDefinition = Builders<T>.Filter.Where(x => x.Id == id);

        return await _mongoCollection.Find(filterDefinition).FirstOrDefaultAsync();
    }

    public T? GetFirstOrDefault(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = GetQuery(filter, orderBy);

        return query.FirstOrDefault();
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = GetQuery(filter, orderBy);

        return await Task.FromResult(query.FirstOrDefault());
    }

    public IEnumerable<T> GetList(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null)
    {
        var query = GetQuery(filter, orderBy, skip, take);

        return query.ToList();
    }

    public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null)
    {
        var query = GetQuery(filter, orderBy, skip, take);

        return await Task.FromResult(query.ToList());
    }

    public void Insert(T entity)
    {
        _mongoCollection.InsertOne(entity);
    }

    public async Task InsertAsync(T entity)
    {
        await _mongoCollection.InsertOneAsync(entity);
    }

    public void InsertRange(IEnumerable<T> entities)
    {
        _mongoCollection.InsertMany(entities);
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities)
    {
        await _mongoCollection.InsertManyAsync(entities);
    }

    public void Update(T entity)
    {
        var filterDefinition = Builders<T>.Filter.Eq(x => x.Id, entity.Id);

        _mongoCollection.ReplaceOne(filterDefinition, entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var filterDefinition = Builders<T>.Filter.Eq(x => x.Id, entity.Id);

        await _mongoCollection.ReplaceOneAsync(filterDefinition, entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
            Update(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
            await UpdateAsync(entity);
    }

    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null)
    {
        IQueryable<T> query = _mongoCollection.AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        if (skip.HasValue && take.HasValue)
            query = query.Skip(skip.Value).Take(take.Value);

        return query;
    }
}