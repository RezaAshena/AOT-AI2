using AOT.Domain.Entities;

namespace AOT.Domain.Interfaces;

public interface IWorkflowRepository
{
    Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(Enums.WorkflowStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IAgentExecutionRepository
{
    Task<AgentExecution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgentExecution>> GetByWorkflowInstanceIdAsync(Guid workflowInstanceId, CancellationToken cancellationToken = default);
    Task AddAsync(AgentExecution agentExecution, CancellationToken cancellationToken = default);
    Task UpdateAsync(AgentExecution agentExecution, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IApprovalRequestRepository
{
    Task<Aggregates.ApprovalRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aggregates.ApprovalRequest>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Aggregates.ApprovalRequest>> GetByRequesterIdAsync(string requesterId, CancellationToken cancellationToken = default);
    Task AddAsync(Aggregates.ApprovalRequest approvalRequest, CancellationToken cancellationToken = default);
    Task UpdateAsync(Aggregates.ApprovalRequest approvalRequest, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface INotificationRepository
{
    Task<NotificationMessage?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationMessage>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
