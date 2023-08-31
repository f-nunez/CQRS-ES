using System.Reflection;
using CQRSES.Query.Application.Common;
using CQRSES.Query.Domain.Entities;
using CQRSES.Query.Infrastructure.Persistence.Contexts;
using CQRSES.Query.Infrastructure.Persistence.Repositories;
using CQRSES.Query.Infrastructure.ServiceBus;
using CQRSES.Query.Infrastructure.ServiceBus.Observers;
using CQRSES.Query.Infrastructure.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureServicesForMassTransit(configuration);

        services.AddInfrastructureServicesForMongoDb(configuration);

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
            mt.AddConsumers(Assembly.GetExecutingAssembly());

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

    private static IServiceCollection AddInfrastructureServicesForMongoDb(
        this IServiceCollection services, IConfiguration configuration)
    {
        BsonClassMap.RegisterClassMap<Ad>();

        services.AddSingleton<IMongoDbContext, MongoDbContext>(sp =>
        {
            return new MongoDbContext(configuration.GetConnectionString("MongoDbConnection"));
        });

        services.AddScoped<IRepository<Ad>, Repository<Ad>>();

        return services;
    }
}