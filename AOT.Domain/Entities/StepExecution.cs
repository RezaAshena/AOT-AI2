using AOT.Domain.Common;
using AOT.Domain.Enums;

namespace AOT.Domain.Entities;

public class StepExecution : BaseEntity
{
    public required string StepName { get; init; }
    public required Guid WorkflowInstanceId { get; init; }
    public StepStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public void Start()
    {
        Status = StepStatus.Running;
        StartedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void Complete(string? output = null)
    {
        Status = StepStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        Output = output;
        MarkAsModified();
    }

    public void Fail(string error)
    {
        Status = StepStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        Error = error;
        MarkAsModified();
    }

    public void Skip()
    {
        Status = StepStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void IncrementRetry()
    {
        RetryCount++;
        MarkAsModified();
    }
}
