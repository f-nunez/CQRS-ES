using System.Text;
using System.Text.Json;
using Contracts.CQRSES;
using CQRSES.Command.Application.Common;
using CQRSES.Command.Domain.Common;
using CQRSES.Command.Infrastructure.Persistence.EventStoreDb;
using CQRSES.Events.AdAggregate;
using CQRSES.Events.Common;
using EventStore.Client;

namespace CQRSES.Command.Infrastructure.EventStores;

public class EventStoreDbEventStore<T> : IEventStore<T> where T : IAggregateRoot
{
    private readonly IEventStoreDbRepository _repository;
    private readonly IServiceBus _serviceBus;

    public EventStoreDbEventStore(IEventStoreDbRepository repository, IServiceBus serviceBus)
    {
        _repository = repository;
        _serviceBus = serviceBus;
    }

    public async Task AppendEventsAsync(T aggregateRoot, long expectedVersion)
    {
        var changes = aggregateRoot.GetChanges();

        if (changes is null || !changes.Any()) return;

        string streamName = aggregateRoot.GetStreamName();

        foreach (var change in changes)
        {
            expectedVersion++;

            change.Version = expectedVersion;

            var preparedEvent = MapEventData(change);

            var integrationEvent = GetIntegrationEvent(change, expectedVersion);

            await _repository.AppendEventAsync(streamName, preparedEvent, expectedVersion - 1);

            await _serviceBus.PublishAsync(integrationEvent);
        }

        aggregateRoot.ClearChanges();
    }

    public async Task<T> ReadStreamEventsAsync<TId>(TId id)
    {
        string streamName = GetStreamName(id);

        var events = await _repository.ReadStreamEventsAsync(streamName);

        var storedEvents = events.Select(Deserialze);

        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

        aggregate.Load(storedEvents);

        return aggregate;
    }

    public static BaseEvent Deserialze(ResolvedEvent resolvedEvent)
    {
        var @event = resolvedEvent.Event;

        string jsonEventData = Encoding.UTF8.GetString(@event.Data.ToArray());

        BaseEvent? data;

        switch (@event.EventType)
        {
            case nameof(AdCreatedV1):
                data = JsonSerializer.Deserialize<AdCreatedV1>(jsonEventData);
                break;
            case nameof(AdDescriptionUpdatedV1):
                data = JsonSerializer.Deserialize<AdDescriptionUpdatedV1>(jsonEventData);
                break;
            case nameof(AdTitleUpdatedV1):
                data = JsonSerializer.Deserialize<AdTitleUpdatedV1>(jsonEventData);
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

    public static EventData MapEventData(BaseEvent @event)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes((object)@event)
        );

        return eventData;
    }
}