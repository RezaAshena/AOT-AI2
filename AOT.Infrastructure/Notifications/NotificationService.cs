using AOT.Application.Services;
using AOT.Domain.Entities;
using AOT.Domain.Enums;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IOperationalMetrics _operationalMetrics;

    public NotificationService(
        INotificationRepository notificationRepository,
        IOperationalMetrics operationalMetrics)
    {
        _notificationRepository = notificationRepository;
        _operationalMetrics = operationalMetrics;
    }

    public async Task<Result<Guid>> NotifyAsync(
        string recipientId,
        string subject,
        string body,
        string idempotencyKey,
        NotificationChannel channel = NotificationChannel.InApp,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken cancellationToken = default)
    {
        var existing = await _notificationRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return Result.Success(existing.Id);
        }

        var notification = new NotificationMessage
        {
            RecipientId = recipientId,
            Subject = subject,
            Body = body,
            Channel = channel,
            Priority = priority,
            IdempotencyKey = idempotencyKey
        };

        const int maxRetries = 3;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await _notificationRepository.AddAsync(notification, cancellationToken);
                await _notificationRepository.SaveChangesAsync(cancellationToken);

                // In-app channel is persisted and considered delivered immediately for MVP.
                notification.MarkSent();
                await _notificationRepository.UpdateAsync(notification, cancellationToken);
                await _notificationRepository.SaveChangesAsync(cancellationToken);
                _operationalMetrics.IncrementNotificationSent(success: true);

                return Result.Success(notification.Id);
            }
            catch (Exception ex)
            {
                notification.MarkFailed(ex.Message);

                if (attempt == maxRetries)
                {
                    await _notificationRepository.UpdateAsync(notification, cancellationToken);
                    await _notificationRepository.SaveChangesAsync(cancellationToken);
                    _operationalMetrics.IncrementNotificationSent(success: false);
                    return Result.Failure<Guid>($"Notification failed after retries: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken);
            }
        }

        return Result.Failure<Guid>("Notification failed unexpectedly.");
    }
}
