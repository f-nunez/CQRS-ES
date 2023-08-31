using CQRSES.Events.Common;

namespace CQRSES.Command.Domain.Common;

public interface IAggregateRoot : IEntity
{
    void ClearChanges();
    IEnumerable<BaseEvent> GetChanges();
    string GetStreamName();
    void Load(IEnumerable<BaseEvent> storedEvents);
}