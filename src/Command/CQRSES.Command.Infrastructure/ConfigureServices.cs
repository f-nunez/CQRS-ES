using CQRSES.Command.Application.Common;
using CQRSES.Command.Domain.AdAggregate;
using CQRSES.Command.Infrastructure.EventStores;
using CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;
using CQRSES.Command.Infrastructure.Persistence.EventStoreDb;
using CQRSES.Command.Infrastructure.Persistence.MongoDb;
using CQRSES.Command.Infrastructure.ServiceBus;
using CQRSES.Command.Infrastructure.ServiceBus.Observers;
using CQRSES.Command.Infrastructure.Settings;
using EventStore.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureServicesForMassTransit(configuration);

        switch (configuration.GetSection(typeof(EventStoreSetting).Name).Get<EventStoreSetting>()!.DatabaseType)
        {
            case DatabaseType.EventStoreDb:
                services.AddInfrastructureServicesForEventStoreDb(configuration);
                break;
            case DatabaseType.MariaDb:
                services.AddInfrastructureServicesForMariaDb(configuration);
                break;
            case DatabaseType.MongoDb:
                services.AddInfrastructureServicesForMongoDb(configuration);
                break;
            case DatabaseType.PostgreSql:
                services.AddInfrastructureServicesForPostgreSql(configuration);
                break;
            case DatabaseType.SqlServer:
                services.AddInfrastructureServicesForSqlServer(configuration);
                break;
        }

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForEventStoreDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IServiceBus, MassTransitServiceBus>();

        services.AddSingleton(
            new EventStoreClient(
                EventStoreClientSettings.Create(
                    configuration.GetConnectionString("EventStoreConnection")!)
            )
        );

        services.AddScoped<IEventStore<Ad>, EventStoreDbEventStore<Ad>>();

        services.AddScoped<IEventStoreDbRepository, EventStoreDbRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForSqlServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EntityFrameworkCoreDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServerConnection"),
                builder => builder.MigrationsAssembly(
                    typeof(EntityFrameworkCoreDbContext).Assembly.FullName
                )
            )
        );

        var dataContext = services.BuildServiceProvider().GetRequiredService<EntityFrameworkCoreDbContext>();

        dataContext.Database.EnsureCreated();

        services.AddScoped<IEventStore<Ad>, EntityFrameworkCoreEventStore<Ad>>();

        services.AddScoped<IEntityFrameworkCoreRepository, EntityFrameworkCoreRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForPostgreSql(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EntityFrameworkCoreDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("PostgreSqlConnection"),
                builder => builder.MigrationsAssembly(
                    typeof(EntityFrameworkCoreDbContext).Assembly.FullName
                )
            )
        );

        var dataContext = services.BuildServiceProvider().GetRequiredService<EntityFrameworkCoreDbContext>();

        dataContext.Database.EnsureCreated();

        services.AddScoped<IEventStore<Ad>, EntityFrameworkCoreEventStore<Ad>>();

        services.AddScoped<IEntityFrameworkCoreRepository, EntityFrameworkCoreRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForMariaDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var version = ServerVersion.AutoDetect(configuration.GetConnectionString("MariaDbConnection"));

        services.AddDbContext<EntityFrameworkCoreDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("MariaDbConnection"),
                version
            )
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
        );

        var dataContext = services.BuildServiceProvider().GetRequiredService<EntityFrameworkCoreDbContext>();

        dataContext.Database.EnsureCreated();

        services.AddScoped<IEventStore<Ad>, EntityFrameworkCoreEventStore<Ad>>();

        services.AddScoped<IEntityFrameworkCoreRepository, EntityFrameworkCoreRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        BsonClassMap.RegisterClassMap<CQRSES.Command.Infrastructure.Persistence.MongoDb.StreamState>();

        services.AddSingleton<IMongoDbContext, MongoDbContext>(sp =>
        {
            return new MongoDbContext(configuration.GetConnectionString("MongoDbConnection"));
        });

        services.AddScoped<IEventStore<Ad>, MongoDbEventStore<Ad>>();

        services.AddScoped<IMongoDbRepository, MongoDbRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServicesForMassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqSetting>(configuration
            .GetSection(typeof(RabbitMqSetting).Name)
            .Get<RabbitMqSetting>()!);

        services.AddScoped<IServiceBus, MassTransitServiceBus>();

        services.AddConsumeObserver<LoggingConsumeObserver>();

        services.AddPublishObserver<LoggingPublishObserver>();

        services.AddReceiveObserver<LoggingReceiveObserver>();

        services.AddSendObserver<LoggingSendObserver>();

        services.AddMassTransit(mt =>
        {
            mt.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSetting = context
                    .GetRequiredService<IRabbitMqSetting>();

                cfg.Host(rabbitMqSetting.HostAddress);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}