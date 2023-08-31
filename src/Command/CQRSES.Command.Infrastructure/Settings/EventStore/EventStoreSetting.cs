namespace CQRSES.Command.Infrastructure.Settings;

public class EventStoreSetting : IEventStoreSetting
{
    public DatabaseType DatabaseType { get; set; } = DatabaseType.EventStoreDb;
}