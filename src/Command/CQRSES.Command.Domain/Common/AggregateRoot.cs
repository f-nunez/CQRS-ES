using CQRSES.Events.Common;

namespace CQRSES.Command.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    readonly List<BaseEvent> _changes = new();
    public int Version { get; private set; } = -1;

    protected override void Apply(BaseEvent @event)
    {
        When(@event);
        _changes.Add(@event);
    }

    public IEnumerable<BaseEvent> GetChanges()
    {
        return _changes.AsEnumerable();
    }

    public string GetStreamName()
    {
        return $"{GetType().Name}-{Id}";
    }

    public void Load(IEnumerable<BaseEvent> storedEvents)
    {
        foreach (var e in storedEvents)
        {
            When(e);
            Version++;
        }
    }

    public void ClearChanges() => _changes.Clear();

    protected void ApplyToEntity(IEntity entity, BaseEvent @event) => entity?.Handle(@event);
}