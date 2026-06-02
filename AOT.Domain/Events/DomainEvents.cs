using AOT.Domain.Common;

namespace AOT.Domain.Events;

public class ApprovalGrantedEvent : BaseDomainEvent
{
    public Guid ApprovalRequestId { get; }
    public string ApproverId { get; }
    public Guid? WorkflowInstanceId { get; }

    public ApprovalGrantedEvent(Guid approvalRequestId, string approverId, Guid? workflowInstanceId)
    {
        ApprovalRequestId = approvalRequestId;
        ApproverId = approverId;
        WorkflowInstanceId = workflowInstanceId;
    }
}

public class ApprovalRejectedEvent : BaseDomainEvent
{
    public Guid ApprovalRequestId { get; }
    public string ApproverId { get; }
    public Guid? WorkflowInstanceId { get; }

    public ApprovalRejectedEvent(Guid approvalRequestId, string approverId, Guid? workflowInstanceId)
    {
        ApprovalRequestId = approvalRequestId;
        ApproverId = approverId;
        WorkflowInstanceId = workflowInstanceId;
    }
}

public class ApprovalEscalatedEvent : BaseDomainEvent
{
    public Guid ApprovalRequestId { get; }
    public Guid? WorkflowInstanceId { get; }

    public ApprovalEscalatedEvent(Guid approvalRequestId, Guid? workflowInstanceId)
    {
        ApprovalRequestId = approvalRequestId;
        WorkflowInstanceId = workflowInstanceId;
    }
}

public class ApprovalExpiredEvent : BaseDomainEvent
{
    public Guid ApprovalRequestId { get; }
    public Guid? WorkflowInstanceId { get; }

    public ApprovalExpiredEvent(Guid approvalRequestId, Guid? workflowInstanceId)
    {
        ApprovalRequestId = approvalRequestId;
        WorkflowInstanceId = workflowInstanceId;
    }
}

public class WorkflowCompletedEvent : BaseDomainEvent
{
    public Guid WorkflowInstanceId { get; }
    public string WorkflowName { get; }
    public TimeSpan Duration { get; }

    public WorkflowCompletedEvent(Guid workflowInstanceId, string workflowName, TimeSpan duration)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowName = workflowName;
        Duration = duration;
    }
}

public class WorkflowFailedEvent : BaseDomainEvent
{
    public Guid WorkflowInstanceId { get; }
    public string WorkflowName { get; }
    public string Error { get; }

    public WorkflowFailedEvent(Guid workflowInstanceId, string workflowName, string error)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowName = workflowName;
        Error = error;
    }
}

public class AgentExecutionCompletedEvent : BaseDomainEvent
{
    public Guid AgentExecutionId { get; }
    public string AgentName { get; }
    public int TokensUsed { get; }
    public TimeSpan Duration { get; }

    public AgentExecutionCompletedEvent(Guid agentExecutionId, string agentName, int tokensUsed, TimeSpan duration)
    {
        AgentExecutionId = agentExecutionId;
        AgentName = agentName;
        TokensUsed = tokensUsed;
        Duration = duration;
    }
}

