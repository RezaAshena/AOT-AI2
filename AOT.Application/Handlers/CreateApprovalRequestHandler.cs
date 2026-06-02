using AOT.Application.Commands;
using AOT.Domain.Aggregates;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class CreateApprovalRequestHandler : IRequestHandler<CreateApprovalRequestCommand, Result<Guid>>
{
    private readonly IApprovalRequestRepository _approvalRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<CreateApprovalRequestHandler> _logger;

    public CreateApprovalRequestHandler(
        IApprovalRequestRepository approvalRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<CreateApprovalRequestHandler> logger)
    {
        _approvalRepository = approvalRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateApprovalRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var approval = new ApprovalRequest
            {
                Title = request.Title,
                Description = request.Description,
                RequesterId = request.RequesterId,
                WorkflowInstanceId = request.WorkflowInstanceId,
                ExpiresAt = request.ExpiresAtUtc
            };

            await _approvalRepository.AddAsync(approval, cancellationToken);
            await _approvalRepository.SaveChangesAsync(cancellationToken);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                EntityType = nameof(ApprovalRequest),
                EntityId = approval.Id,
                Action = "Created",
                PerformedBy = request.RequesterId,
                Details = request.Description
            }, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created approval request {ApprovalId}", approval.Id);
            return Result.Success(approval.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create approval request for {RequesterId}", request.RequesterId);
            return Result.Failure<Guid>($"Failed to create approval request: {ex.Message}");
        }
    }
}
