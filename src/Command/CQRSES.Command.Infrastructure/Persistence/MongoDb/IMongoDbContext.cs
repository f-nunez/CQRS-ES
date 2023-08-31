using MongoDB.Driver;

namespace CQRSES.Command.Infrastructure.Persistence.MongoDb;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>();
}