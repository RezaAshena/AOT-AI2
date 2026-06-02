using AOT.Domain.Common;
using AOT.Domain.Enums;

namespace AOT.Domain.Entities;

public class NotificationMessage : BaseEntity
{
    public required string RecipientId { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public NotificationChannel Channel { get; init; } = NotificationChannel.InApp;
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public required string IdempotencyKey { get; init; }
    public DateTime? SentAt { get; private set; }
    public bool IsSent { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    public void MarkSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
        Error = null;
        MarkAsModified();
    }

    public void MarkFailed(string error)
    {
        IsSent = false;
        RetryCount++;
        Error = error;
        MarkAsModified();
    }
}
