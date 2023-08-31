namespace CQRSES.Command.Infrastructure.Settings;

public interface IEventStoreSetting
{
    DatabaseType DatabaseType { get; }
}