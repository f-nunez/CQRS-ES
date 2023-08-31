namespace CQRSES.Command.Infrastructure.Settings;

public interface IRabbitMqSetting
{
    Uri HostAddress { get; }
}