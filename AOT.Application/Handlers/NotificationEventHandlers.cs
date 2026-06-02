using AOT.Application.Services;
using AOT.Domain.Events;
using Microsoft.Extensions.Logging;

namespace AOT.Application.Handlers;

public class ApprovalGrantedEventHandler : INotificationHandler<ApprovalGrantedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ApprovalGrantedEventHandler> _logger;

    public ApprovalGrantedEventHandler(INotificationService notificationService, ILogger<ApprovalGrantedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(ApprovalGrantedEvent notification, CancellationToken cancellationToken)
    {
        var idempotencyKey = $"approval-granted:{notification.ApprovalRequestId}";
        await _notificationService.NotifyAsync(
            recipientId: notification.ApproverId,
            subject: "Approval Granted",
            body: $"Approval request {notification.ApprovalRequestId} has been granted.",
            idempotencyKey: idempotencyKey,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Processed ApprovalGrantedEvent for {ApprovalRequestId}", notification.ApprovalRequestId);
    }
}

public class ApprovalRejectedEventHandler : INotificationHandler<ApprovalRejectedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ApprovalRejectedEventHandler> _logger;

    public ApprovalRejectedEventHandler(INotificationService notificationService, ILogger<ApprovalRejectedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(ApprovalRejectedEvent notification, CancellationToken cancellationToken)
    {
        var idempotencyKey = $"approval-rejected:{notification.ApprovalRequestId}";
        await _notificationService.NotifyAsync(
            recipientId: notification.ApproverId,
            subject: "Approval Rejected",
            body: $"Approval request {notification.ApprovalRequestId} has been rejected.",
            idempotencyKey: idempotencyKey,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Processed ApprovalRejectedEvent for {ApprovalRequestId}", notification.ApprovalRequestId);
    }
}
