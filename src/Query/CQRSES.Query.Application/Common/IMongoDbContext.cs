using MongoDB.Driver;

namespace CQRSES.Query.Application.Common;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>();
}