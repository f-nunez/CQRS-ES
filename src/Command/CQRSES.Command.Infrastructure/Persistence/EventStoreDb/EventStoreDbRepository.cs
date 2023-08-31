using EventStore.Client;

namespace CQRSES.Command.Infrastructure.Persistence.EventStoreDb;

public class EventStoreDbRepository : IEventStoreDbRepository
{
    private readonly EventStoreClient _eventStoreClient;

    public EventStoreDbRepository(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task AppendEventAsync(string streamName, EventData eventData, long expectedVersion)
    {
        try
        {
            await _eventStoreClient.AppendToStreamAsync(
                streamName,
                (ulong)expectedVersion,
                new EventData[] { eventData }
            );
        }
        catch (WrongExpectedVersionException ex)
        {
            throw new Exception($"Append failed due to expected version. Stream: {streamName}, Expected version: {ex.ExpectedVersion}, Actual version: {ex.ActualVersion}");
        }
    }

    public async Task<List<ResolvedEvent>> ReadStreamEventsAsync(string streamName)
    {
        var result = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start
        );
        
        return await result.ToListAsync();
    }
}