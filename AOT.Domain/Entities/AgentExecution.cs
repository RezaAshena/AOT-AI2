using AOT.Domain.Common;
using AOT.Domain.Enums;

namespace AOT.Domain.Entities;

public class AgentExecution : BaseEntity
{
    public required string AgentName { get; init; }
    public required string AgentType { get; init; }
    public Guid? WorkflowInstanceId { get; init; }
    public Guid? StepExecutionId { get; init; }
    public AgentExecutionStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Request { get; set; }
    public string? Response { get; set; }
    public string? Error { get; private set; }
    public int TokensUsed { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue 
        ? CompletedAt.Value - StartedAt.Value 
        : null;

    public void Start()
    {
        Status = AgentExecutionStatus.Running;
        StartedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void Complete(string? response = null, int tokensUsed = 0)
    {
        Status = AgentExecutionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Response = response;
        TokensUsed = tokensUsed;
        MarkAsModified();
    }

    public void Fail(string error)
    {
        Status = AgentExecutionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        Error = error;
        MarkAsModified();
    }

    public void Timeout()
    {
        Status = AgentExecutionStatus.TimedOut;
        CompletedAt = DateTime.UtcNow;
        Error = "Agent execution timed out";
        MarkAsModified();
    }
}
