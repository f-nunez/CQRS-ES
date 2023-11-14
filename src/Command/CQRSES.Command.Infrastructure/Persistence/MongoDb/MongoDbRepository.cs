using MongoDB.Driver;

namespace CQRSES.Command.Infrastructure.Persistence.MongoDb;

public class MongoDbRepository : IMongoDbRepository
{
    private readonly IMongoCollection<StreamState> _mongoCollection;

    public MongoDbRepository(IMongoDbContext mongoDbContext)
    {
        _mongoCollection = mongoDbContext.GetCollection<StreamState>();
    }

    public async Task AppendEventAsync(StreamState streamState)
    {
        await _mongoCollection
            .InsertOneAsync(streamState)
            .ConfigureAwait(false);
    }

    public async Task<List<StreamState>?> ReadStreamEventsAsync(string streamName)
    {
        return await _mongoCollection
            .Find(x => x.StreamName == streamName)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}