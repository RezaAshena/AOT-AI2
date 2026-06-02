using AOT.Application.Commands;
using AOT.Domain.Aggregates;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Application.Services;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand, Result>
{
    private readonly IApprovalRequestRepository _repository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IOperationalMetrics _operationalMetrics;
    private readonly IPublisher _publisher;
    private readonly ILogger<ApproveRequestCommandHandler> _logger;

    public ApproveRequestCommandHandler(
        IApprovalRequestRepository repository,
        IWorkflowRepository workflowRepository,
        IAuditLogRepository auditLogRepository,
        IOperationalMetrics operationalMetrics,
        IPublisher publisher,
        ILogger<ApproveRequestCommandHandler> logger)
    {
        _repository = repository;
        _workflowRepository = workflowRepository;
        _auditLogRepository = auditLogRepository;
        _operationalMetrics = operationalMetrics;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result> Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var approvalRequest = await _repository.GetByIdAsync(request.ApprovalRequestId, cancellationToken);
            if (approvalRequest == null)
                return Result.Failure("Approval request not found");

            approvalRequest.Approve(request.ApproverId, request.Comments);
            await _repository.UpdateAsync(approvalRequest, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            if (approvalRequest.WorkflowInstanceId is Guid workflowId)
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken);
                if (workflow is not null && workflow.Status == Domain.Enums.WorkflowStatus.NotStarted)
                {
                    workflow.Start();
                    await _workflowRepository.UpdateAsync(workflow, cancellationToken);
                    await _workflowRepository.SaveChangesAsync(cancellationToken);
                }
            }

            await _auditLogRepository.AddAsync(new AuditLog
            {
                EntityType = nameof(ApprovalRequest),
                EntityId = request.ApprovalRequestId,
                Action = "Approved",
                PerformedBy = request.ApproverId,
                Details = request.Comments
            }, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in approvalRequest.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }

            approvalRequest.ClearDomainEvents();
            _operationalMetrics.IncrementApprovalAction("approved");

            _logger.LogInformation("Approval {ApprovalId} granted by {ApproverId}", 
                request.ApprovalRequestId, request.ApproverId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve request {ApprovalId}", request.ApprovalRequestId);
            return Result.Failure($"Failed to approve request: {ex.Message}");
        }
    }
}

public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, Result>
{
    private readonly IApprovalRequestRepository _repository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IOperationalMetrics _operationalMetrics;
    private readonly IPublisher _publisher;
    private readonly ILogger<RejectRequestCommandHandler> _logger;

    public RejectRequestCommandHandler(
        IApprovalRequestRepository repository,
        IWorkflowRepository workflowRepository,
        IAuditLogRepository auditLogRepository,
        IOperationalMetrics operationalMetrics,
        IPublisher publisher,
        ILogger<RejectRequestCommandHandler> logger)
    {
        _repository = repository;
        _workflowRepository = workflowRepository;
        _auditLogRepository = auditLogRepository;
        _operationalMetrics = operationalMetrics;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var approvalRequest = await _repository.GetByIdAsync(request.ApprovalRequestId, cancellationToken);
            if (approvalRequest == null)
                return Result.Failure("Approval request not found");

            approvalRequest.Reject(request.ApproverId, request.Comments);
            await _repository.UpdateAsync(approvalRequest, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            if (approvalRequest.WorkflowInstanceId is Guid workflowId)
            {
                var workflow = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken);
                if (workflow is not null && workflow.Status is not Domain.Enums.WorkflowStatus.Completed and not Domain.Enums.WorkflowStatus.Failed and not Domain.Enums.WorkflowStatus.Cancelled)
                {
                    workflow.Cancel();
                    await _workflowRepository.UpdateAsync(workflow, cancellationToken);
                    await _workflowRepository.SaveChangesAsync(cancellationToken);
                }
            }

            await _auditLogRepository.AddAsync(new AuditLog
            {
                EntityType = nameof(ApprovalRequest),
                EntityId = request.ApprovalRequestId,
                Action = "Rejected",
                PerformedBy = request.ApproverId,
                Details = request.Comments
            }, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in approvalRequest.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }

            approvalRequest.ClearDomainEvents();
            _operationalMetrics.IncrementApprovalAction("rejected");

            _logger.LogInformation("Approval {ApprovalId} rejected by {ApproverId}", 
                request.ApprovalRequestId, request.ApproverId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reject request {ApprovalId}", request.ApprovalRequestId);
            return Result.Failure($"Failed to reject request: {ex.Message}");
        }
    }
}
