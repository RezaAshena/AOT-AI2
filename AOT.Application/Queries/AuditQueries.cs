using AOT.Domain.Entities;
using AOT.Shared.Results;

namespace AOT.Application.Queries;

public record GetAuditLogsQuery(string EntityType, Guid EntityId) : IRequest<Result<IEnumerable<AuditLog>>>;
