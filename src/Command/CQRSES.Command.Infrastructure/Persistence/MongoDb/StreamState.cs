using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRSES.Command.Infrastructure.Persistence.MongoDb;

public class StreamState
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string EventPayload { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string StreamName { get; set; } = null!;
    public long Version { get; set; }
}