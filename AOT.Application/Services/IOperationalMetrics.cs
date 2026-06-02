namespace AOT.Application.Services;

public interface IOperationalMetrics
{
    void IncrementWorkflowStarted();
    void IncrementApprovalAction(string action);
    void IncrementNotificationSent(bool success);
    OperationalSnapshot Snapshot();
}

public sealed record OperationalSnapshot(
    long WorkflowsStarted,
    long ApprovalsApproved,
    long ApprovalsRejected,
    long NotificationsSent,
    long NotificationsFailed,
    DateTime CapturedAtUtc);
