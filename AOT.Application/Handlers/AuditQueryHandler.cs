using AOT.Application.Queries;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<IEnumerable<AuditLog>>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<GetAuditLogsQueryHandler> _logger;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository, ILogger<GetAuditLogsQueryHandler> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AuditLog>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var logs = await _auditLogRepository.GetByEntityAsync(request.EntityType, request.EntityId, cancellationToken);
            return Result.Success(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit logs for {EntityType}:{EntityId}", request.EntityType, request.EntityId);
            return Result.Failure<IEnumerable<AuditLog>>($"Failed to get audit logs: {ex.Message}");
        }
    }
}
