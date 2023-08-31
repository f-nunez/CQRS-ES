namespace CQRSES.Query.Infrastructure.Settings;

public class RabbitMqSetting : IRabbitMqSetting
{
    public Uri HostAddress { get; set; } = null!;
}