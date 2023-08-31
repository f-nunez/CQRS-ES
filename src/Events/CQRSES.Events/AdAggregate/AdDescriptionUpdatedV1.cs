using CQRSES.Events.Common;

namespace CQRSES.Events.AdAggregate;

public class AdDescriptionUpdatedV1 : BaseEvent
{
    public string? Description { get; set; }
}