using CQRSES.Query.Application.Common;
using MassTransit;

namespace CQRSES.Query.Infrastructure.ServiceBus;

public class MassTransitServiceBus : IServiceBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitServiceBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        await _publishEndpoint.Publish(message, cancellationToken);
    }
}