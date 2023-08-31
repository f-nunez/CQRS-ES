namespace CQRSES.Command.Infrastructure.Settings;

public class RabbitMqSetting : IRabbitMqSetting
{
    public Uri HostAddress { get; set; } = null!;
}