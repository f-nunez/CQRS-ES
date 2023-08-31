using CQRSES.Events.Common;

namespace CQRSES.Command.Domain.Common;

public interface IEntity
{
    void Handle(BaseEvent @event);
}