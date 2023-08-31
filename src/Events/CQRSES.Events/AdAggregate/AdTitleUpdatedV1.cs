using CQRSES.Events.Common;

namespace CQRSES.Events.AdAggregate;

public class AdTitleUpdatedV1 : BaseEvent
{
    public string? Title { get; set; }
}