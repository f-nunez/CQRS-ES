using CQRSES.Command.Domain.Common;
using CQRSES.Events.AdAggregate;
using CQRSES.Events.Common;

namespace CQRSES.Command.Domain.AdAggregate;

public class Ad : AggregateRoot<string>
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public void Create(string id, string title, string description)
    {
        Apply(
            new AdCreatedV1
            {
                Id = id,
                Title = title,
                Description = description
            }
        );
    }

    public void UpdateDescription(string description)
    {
        Apply(
            new AdDescriptionUpdatedV1
            {
                Id = Id,
                Description = description
            }
        );
    }

    public void UpdateTitle(string title)
    {
        Apply(
            new AdTitleUpdatedV1
            {
                Id = Id,
                Title = title
            }
        );
    }

    protected override void When(BaseEvent @event)
    {
        switch (@event)
        {
            case AdCreatedV1 e:
                Id = e.Id;
                Title = e.Title;
                Description = e.Description;
                break;
            case AdDescriptionUpdatedV1 e:
                Id = e.Id;
                Description = e.Description;
                break;
            case AdTitleUpdatedV1 e:
                Id = e.Id;
                Title = e.Title;
                break;
            default:
                throw new ArgumentNullException(nameof(@event), $"When method not found the event for {@event.GetType().Name}");
        }
    }
}