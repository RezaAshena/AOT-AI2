using AOT.Shared.Results;

namespace AOT.Application.Commands;

public record CreateApprovalRequestCommand : IRequest<Result<Guid>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string RequesterId { get; init; }
    public Guid? WorkflowInstanceId { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}
