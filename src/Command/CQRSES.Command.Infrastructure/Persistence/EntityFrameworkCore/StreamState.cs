namespace CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;

public class StreamState
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string EventPayload { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string StreamName { get; set; } = null!;
    public long Version { get; set; }
}