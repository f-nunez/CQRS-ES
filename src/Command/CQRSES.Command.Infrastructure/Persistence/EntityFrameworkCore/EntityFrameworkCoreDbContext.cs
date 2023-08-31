using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;

public class EntityFrameworkCoreDbContext : DbContext
{
    public EntityFrameworkCoreDbContext(DbContextOptions<EntityFrameworkCoreDbContext> options) : base(options)
    {
    }

    public DbSet<StreamState> StreamStates => Set<StreamState>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}