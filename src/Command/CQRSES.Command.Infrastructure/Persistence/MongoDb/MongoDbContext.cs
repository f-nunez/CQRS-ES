using MongoDB.Driver;

namespace CQRSES.Command.Infrastructure.Persistence.MongoDb;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _mongoDatabase;

    public MongoDbContext(string? connectionString)
    {
        var mongoClient = new MongoClient(connectionString);

        string databaseName = mongoClient.ListDatabaseNames().First();

        _mongoDatabase = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return _mongoDatabase.GetCollection<T>(typeof(T).Name);
    }
}