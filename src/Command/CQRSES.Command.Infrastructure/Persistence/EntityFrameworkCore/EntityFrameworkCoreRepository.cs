using Microsoft.EntityFrameworkCore;

namespace CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;

public class EntityFrameworkCoreRepository : IEntityFrameworkCoreRepository
{
    private readonly EntityFrameworkCoreDbContext _dbContext;

    public EntityFrameworkCoreRepository(EntityFrameworkCoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AppendEventAsync(StreamState streamState)
    {
        await _dbContext.AddAsync(streamState);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<StreamState>> ReadStreamEventsAsync(string streamName)
    {
        var query = from streamState in _dbContext.StreamStates
                    where streamState.StreamName == streamName
                    orderby streamState.Version
                    select streamState;

        return await query.AsNoTracking().ToListAsync();
    }
}