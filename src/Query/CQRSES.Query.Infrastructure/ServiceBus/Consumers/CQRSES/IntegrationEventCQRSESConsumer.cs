using System.Text.Json;
using Contracts.CQRSES;
using CQRSES.Events.AdAggregate;
using CQRSES.Query.Application.Common;
using CQRSES.Query.Domain.Entities;
using MassTransit;

namespace CQRSES.Query.Infrastructure.ServiceBus.Consumers;

public class IntegrationEventCQRSESConsumer : IConsumer<EventCQRSESContract>
{
    private readonly IRepository<Ad> _repository;

    public IntegrationEventCQRSESConsumer(IRepository<Ad> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<EventCQRSESContract> context)
    {
        var integrationEvent = context.Message;

        switch (integrationEvent.EventType)
        {
            case nameof(AdCreatedV1):
                var adCreatedV1 = JsonSerializer.Deserialize<AdCreatedV1>(integrationEvent.EventPayload!)!;
                var ad = new Ad
                {
                    Id = adCreatedV1.Id,
                    Description = adCreatedV1.Description,
                    IsActive = true,
                    Title = adCreatedV1.Title,
                    Version = integrationEvent.EventVersion
                };
                await _repository.InsertAsync(ad);
                break;
            case nameof(AdDescriptionUpdatedV1):
                var adDescriptionUpdatedV1 = JsonSerializer.Deserialize<AdDescriptionUpdatedV1>(integrationEvent.EventPayload!)!;
                var adForDescription = await _repository.GetByIdAsync(adDescriptionUpdatedV1.Id!);
                adForDescription.Description = adDescriptionUpdatedV1.Description;
                adForDescription.Version = integrationEvent.EventVersion;
                await _repository.UpdateAsync(adForDescription);
                break;
            case nameof(AdTitleUpdatedV1):
                var adTitleUpdatedV1 = JsonSerializer.Deserialize<AdTitleUpdatedV1>(integrationEvent.EventPayload!)!;
                var adForTitle = await _repository.GetByIdAsync(adTitleUpdatedV1.Id!);
                adForTitle.Title = adTitleUpdatedV1.Title;
                adForTitle.Version = integrationEvent.EventVersion;
                await _repository.UpdateAsync(adForTitle);
                break;
            default:
                throw new ArgumentNullException(nameof(integrationEvent.EventType), $"Event not found for {integrationEvent.EventType}");
        }
    }
}