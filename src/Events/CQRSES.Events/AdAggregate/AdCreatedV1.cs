using CQRSES.Events.Common;

namespace CQRSES.Events.AdAggregate;

public class AdCreatedV1 : BaseEvent
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}