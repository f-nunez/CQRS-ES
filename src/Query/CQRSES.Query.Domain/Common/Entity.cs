using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRSES.Query.Domain.Common;

public abstract class Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }
    public bool IsActive { get; set; }
    public long Version { get; set; }
}