using EventStore.Client;

namespace CQRSES.Command.Infrastructure.Persistence.EventStoreDb;

public interface IEventStoreDbRepository
{
    Task AppendEventAsync(string streamName, EventData eventData, long expectedVersion);
    Task<List<ResolvedEvent>> ReadStreamEventsAsync(string streamName);
}