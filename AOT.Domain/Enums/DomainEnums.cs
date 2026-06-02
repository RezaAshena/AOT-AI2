namespace AOT.Domain.Enums;

public enum WorkflowStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Cancelled,
    Paused
}

public enum StepStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Skipped
}

public enum AgentExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    TimedOut
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    Escalated,
    Expired
}

public enum NotificationChannel
{
    InApp,
    Email,
    SMS,
    Teams,
    Slack
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}
