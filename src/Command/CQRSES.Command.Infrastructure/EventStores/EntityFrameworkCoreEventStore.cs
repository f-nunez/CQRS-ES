using System.Text.Json;
using Contracts.CQRSES;
using CQRSES.Command.Application.Common;
using CQRSES.Command.Domain.Common;
using CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;
using CQRSES.Events.AdAggregate;
using CQRSES.Events.Common;

namespace CQRSES.Command.Infrastructure.EventStores;

public class EntityFrameworkCoreEventStore<T> : IEventStore<T> where T : IAggregateRoot
{
    private readonly IEntityFrameworkCoreRepository _repository;
    private readonly IServiceBus _serviceBus;

    public EntityFrameworkCoreEventStore(IEntityFrameworkCoreRepository repository, IServiceBus serviceBus)
    {
        _repository = repository;
        _serviceBus = serviceBus;
    }

    public async Task AppendEventsAsync(T aggregateRoot, long expectedVersion)
    {
        var changes = aggregateRoot.GetChanges();

        if (changes is null || !changes.Any()) return;

        string streamName = aggregateRoot.GetStreamName();

        var events = await _repository.ReadStreamEventsAsync(streamName);

        if (expectedVersion != -1 && events[^1].Version != expectedVersion)
            throw new Exception($"Append failed due to expected version. Stream: {streamName}, Expected version: {expectedVersion}, Actual version: {events[^1].Version}");

        foreach (var change in changes)
        {
            expectedVersion++;

            change.Version = expectedVersion;

            var streamState = MapStreamState(change, streamName, expectedVersion);

            var integrationEvent = GetIntegrationEvent(change, expectedVersion);

            await _repository.AppendEventAsync(streamState);

            await _serviceBus.PublishAsync(integrationEvent);
        }

        aggregateRoot.ClearChanges();
    }

    public async Task<T> ReadStreamEventsAsync<TId>(TId id)
    {
        string streamName = GetStreamName(id);

        var events = await _repository.ReadStreamEventsAsync(streamName);

        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

        var storedEvents = events.Select(Deserialze);

        aggregate.Load(storedEvents);

        return aggregate;
    }

    static BaseEvent Deserialze(StreamState @event)
    {
        BaseEvent? data;

        switch (@event.EventType)
        {
            case nameof(AdCreatedV1):
                data = JsonSerializer.Deserialize<AdCreatedV1>(@event.EventPayload);
                break;
            case nameof(AdDescriptionUpdatedV1):
                data = JsonSerializer.Deserialize<AdDescriptionUpdatedV1>(@event.EventPayload);
                break;
            case nameof(AdTitleUpdatedV1):
                data = JsonSerializer.Deserialize<AdTitleUpdatedV1>(@event.EventPayload);
                break;
            default:
                throw new ArgumentNullException(nameof(@event.EventType), $"Not found the event type for {@event.EventType}");
        }

        if (data is null)
            throw new ArgumentNullException(nameof(data), $"Cannot deserialize the event type for {@event.EventType}");

        return data;
    }

    static EventCQRSESContract GetIntegrationEvent(BaseEvent @event, long version)
    {
        var integrationEvent = new EventCQRSESContract
        {
            EventPayload = JsonSerializer.Serialize((object)@event),
            EventType = @event.GetType().Name,
            EventVersion = version
        };

        return integrationEvent;
    }

    static string GetStreamName<TId>(TId aggregateId)
    {
        if (aggregateId == null)
            throw new ArgumentNullException(nameof(aggregateId));

        return $"{typeof(T).Name}-{aggregateId}";
    }

    static StreamState MapStreamState(BaseEvent @event, string streamName, long version)
    {
        var eventData = new StreamState
        {
            CreatedOn = DateTime.UtcNow,
            EventPayload = JsonSerializer.Serialize((object)@event),
            EventType = @event.GetType().Name,
            StreamName = streamName,
            Version = version
        };

        return eventData;
    }
}