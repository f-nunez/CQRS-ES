using MassTransit;

namespace CQRSES.Query.Infrastructure.ServiceBus.Consumers;

public class IntegrationEventCQRSESConsumerDefinition : ConsumerDefinition<IntegrationEventCQRSESConsumer>
{
    private static readonly int ConcurrencyLimit = 1;
    private static readonly int[] RetryInterval = { 100, 1000, 2000, 5000 };

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<IntegrationEventCQRSESConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseConcurrencyLimit(ConcurrencyLimit);

        endpointConfigurator.UseMessageRetry(r => r.Intervals(RetryInterval));

        endpointConfigurator.UseInMemoryOutbox(context);
    }
}