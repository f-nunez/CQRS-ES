namespace CQRSES.Events.Common;

public abstract class BaseEvent
{
    public string? Id { get; set; }
    public long Version { get; set; }
}