using AOT.Domain.Common;
using AOT.Domain.Enums;

namespace AOT.Domain.Entities;

public class WorkflowInstance : BaseEntity
{
    public required string WorkflowName { get; init; }
    public required string WorkflowDefinitionId { get; init; }
    public WorkflowStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Error { get; private set; }
    public Dictionary<string, object> Context { get; private set; } = new();
    public List<StepExecution> Steps { get; private set; } = new();

    public void Start()
    {
        if (Status != WorkflowStatus.NotStarted)
            throw new InvalidOperationException($"Cannot start workflow in {Status} status.");

        Status = WorkflowStatus.Running;
        StartedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void Complete()
    {
        if (Status != WorkflowStatus.Running)
            throw new InvalidOperationException($"Cannot complete workflow in {Status} status.");

        Status = WorkflowStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void Fail(string error)
    {
        Status = WorkflowStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        Error = error;
        MarkAsModified();
    }

    public void Cancel()
    {
        if (Status == WorkflowStatus.Completed || Status == WorkflowStatus.Failed)
            throw new InvalidOperationException($"Cannot cancel workflow in {Status} status.");

        Status = WorkflowStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        MarkAsModified();
    }

    public void AddStep(StepExecution step)
    {
        Steps.Add(step);
        MarkAsModified();
    }

    public void SetContext(Dictionary<string, object> context)
    {
        Context = context;
        MarkAsModified();
    }
}
