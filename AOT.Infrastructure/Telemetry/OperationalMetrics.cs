using System.Diagnostics.Metrics;
using AOT.Application.Services;

namespace AOT.Infrastructure.Telemetry;

public class OperationalMetrics : IOperationalMetrics, IDisposable
{
    private static readonly Meter Meter = new("AOT-AI.Operations", "1.0.0");
    private readonly Counter<long> _workflowStartedCounter = Meter.CreateCounter<long>("workflow.started");
    private readonly Counter<long> _approvalActionCounter = Meter.CreateCounter<long>("approval.actions");
    private readonly Counter<long> _notificationCounter = Meter.CreateCounter<long>("notifications.sent");

    private long _workflowsStarted;
    private long _approvalsApproved;
    private long _approvalsRejected;
    private long _notificationsSent;
    private long _notificationsFailed;

    public void IncrementWorkflowStarted()
    {
        Interlocked.Increment(ref _workflowsStarted);
        _workflowStartedCounter.Add(1);
    }

    public void IncrementApprovalAction(string action)
    {
        _approvalActionCounter.Add(1, new KeyValuePair<string, object?>("action", action));
        if (string.Equals(action, "approved", StringComparison.OrdinalIgnoreCase))
        {
            Interlocked.Increment(ref _approvalsApproved);
        }
        else if (string.Equals(action, "rejected", StringComparison.OrdinalIgnoreCase))
        {
            Interlocked.Increment(ref _approvalsRejected);
        }
    }

    public void IncrementNotificationSent(bool success)
    {
        _notificationCounter.Add(1, new KeyValuePair<string, object?>("success", success));
        if (success)
        {
            Interlocked.Increment(ref _notificationsSent);
        }
        else
        {
            Interlocked.Increment(ref _notificationsFailed);
        }
    }

    public OperationalSnapshot Snapshot() => new(
        WorkflowsStarted: Interlocked.Read(ref _workflowsStarted),
        ApprovalsApproved: Interlocked.Read(ref _approvalsApproved),
        ApprovalsRejected: Interlocked.Read(ref _approvalsRejected),
        NotificationsSent: Interlocked.Read(ref _notificationsSent),
        NotificationsFailed: Interlocked.Read(ref _notificationsFailed),
        CapturedAtUtc: DateTime.UtcNow);

    public void Dispose()
    {
        Meter.Dispose();
    }
}
