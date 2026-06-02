using AOT.Domain.Common;
using AOT.Domain.Enums;
using AOT.Domain.Events;

namespace AOT.Domain.Aggregates;

public class ApprovalRequest : BaseEntity
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string RequesterId { get; init; }
    public string? ApproverId { get; private set; }
    public ApprovalStatus Status { get; private set; } = ApprovalStatus.Pending;
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovalComments { get; private set; }
    public Guid? WorkflowInstanceId { get; init; }
    public DateTime ExpiresAt { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void Approve(string approverId, string? comments = null)
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException($"Cannot approve request in {Status} status.");

        if (DateTime.UtcNow > ExpiresAt)
        {
            Expire();
            throw new InvalidOperationException("Approval request has expired.");
        }

        Status = ApprovalStatus.Approved;
        ApproverId = approverId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
        MarkAsModified(approverId);

        _domainEvents.Add(new ApprovalGrantedEvent(Id, approverId, WorkflowInstanceId));
    }

    public void Reject(string approverId, string? comments = null)
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException($"Cannot reject request in {Status} status.");

        Status = ApprovalStatus.Rejected;
        ApproverId = approverId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
        MarkAsModified(approverId);

        _domainEvents.Add(new ApprovalRejectedEvent(Id, approverId, WorkflowInstanceId));
    }

    public void Escalate()
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException($"Cannot escalate request in {Status} status.");

        Status = ApprovalStatus.Escalated;
        MarkAsModified();

        _domainEvents.Add(new ApprovalEscalatedEvent(Id, WorkflowInstanceId));
    }

    public void Expire()
    {
        if (Status != ApprovalStatus.Pending)
            return;

        Status = ApprovalStatus.Expired;
        MarkAsModified();

        _domainEvents.Add(new ApprovalExpiredEvent(Id, WorkflowInstanceId));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
