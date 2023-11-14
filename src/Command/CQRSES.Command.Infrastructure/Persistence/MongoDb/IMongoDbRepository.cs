namespace CQRSES.Command.Infrastructure.Persistence.MongoDb;

public interface IMongoDbRepository
{
    Task AppendEventAsync(StreamState streamState);
    Task<List<StreamState>?> ReadStreamEventsAsync(string streamName);
}