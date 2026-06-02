using AOT.Domain.Common;

namespace AOT.Domain.Entities;

public class AuditLog : BaseEntity
{
    public required string EntityType { get; init; }
    public required Guid EntityId { get; init; }
    public required string Action { get; init; }
    public required string PerformedBy { get; init; }
    public string? Details { get; init; }
}
