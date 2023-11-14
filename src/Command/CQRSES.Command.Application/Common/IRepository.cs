using CQRSES.Command.Domain.Common;

namespace CQRSES.Command.Application.Common;

public interface IRepository<T> where T : IAggregateRoot
{
    Task AppendEventsAsync(T aggregateRoot, long expectedVersion);
    Task<T?> ReadStreamEventsAsync<TId>(TId id);
}