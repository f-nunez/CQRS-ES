namespace Contracts.CQRSES;

public class EventCQRSESContract
{
    public Guid CausationId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? EventPayload { get; set; }
    public string? EventType { get; set; }
    public long EventVersion { get; set; }
    public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
}