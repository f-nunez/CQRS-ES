namespace CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;

public interface IEntityFrameworkCoreRepository
{
    Task AppendEventAsync(StreamState streamState);
    Task<List<StreamState>?> ReadStreamEventsAsync(string streamName);
}