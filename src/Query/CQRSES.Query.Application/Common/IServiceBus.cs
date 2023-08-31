namespace CQRSES.Query.Application.Common;

public interface IServiceBus
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}