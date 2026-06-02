using AOT.Domain.Enums;
using AOT.Shared.Results;

namespace AOT.Application.Services;

public interface INotificationService
{
    Task<Result<Guid>> NotifyAsync(
        string recipientId,
        string subject,
        string body,
        string idempotencyKey,
        NotificationChannel channel = NotificationChannel.InApp,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken cancellationToken = default);
}
