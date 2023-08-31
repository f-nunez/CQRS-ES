using CQRSES.Events.Common;

namespace CQRSES.Command.Domain.Common;

public abstract class Entity<TId> : IEntity
{
    readonly Action<object> _applier = null!;
    public TId? Id { get; set; }
    public bool IsActive { get; set; } = true;

    public void Handle(BaseEvent @event)
    {
        When(@event);
    }

    protected abstract void When(BaseEvent @event);

    protected virtual void Apply(BaseEvent @event)
    {
        When(@event);
        _applier(@event);
    }
}