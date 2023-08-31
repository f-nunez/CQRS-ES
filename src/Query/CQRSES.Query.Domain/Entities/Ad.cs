using CQRSES.Query.Domain.Common;

namespace CQRSES.Query.Domain.Entities;

public class Ad : Entity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}