using AOT.Shared.Results;

namespace AOT.Application.Commands;

public record CreateWorkflowCommand : IRequest<Result<Guid>>
{
    public required string WorkflowName { get; init; }
    public required string WorkflowDefinitionId { get; init; }
    public Dictionary<string, object> InitialContext { get; init; } = new();
}

public record StartWorkflowCommand : IRequest<Result>
{
    public required Guid WorkflowInstanceId { get; init; }
}

public record ApproveRequestCommand : IRequest<Result>
{
    public required Guid ApprovalRequestId { get; init; }
    public required string ApproverId { get; init; }
    public string? Comments { get; init; }
}

public record RejectRequestCommand : IRequest<Result>
{
    public required Guid ApprovalRequestId { get; init; }
    public required string ApproverId { get; init; }
    public string? Comments { get; init; }
}
