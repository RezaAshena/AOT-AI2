using AOT.Application.Commands;
using AOT.Application.Handlers;
using AOT.Domain.Aggregates;
using AOT.Domain.Interfaces;
using AOT.Domain.Enums;
using AOT.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace AOT.Tests.Unit.Application;

public class ApproveRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRequestExists_ReturnsSuccess()
    {
        var approval = new ApprovalRequest
        {
            Title = "Approve",
            Description = "desc",
            RequesterId = "req-1",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        var approvalRepo = new Mock<IApprovalRequestRepository>();
        approvalRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(approval);
        approvalRepo.Setup(r => r.UpdateAsync(It.IsAny<ApprovalRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        approvalRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var workflowRepo = new Mock<IWorkflowRepository>();
        workflowRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AOT.Domain.Entities.WorkflowInstance?)null);

        var auditRepo = new Mock<IAuditLogRepository>();
        auditRepo.Setup(r => r.AddAsync(It.IsAny<AOT.Domain.Entities.AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        auditRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var metrics = new Mock<IOperationalMetrics>();
        var publisher = new Mock<IPublisher>();
        publisher.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var logger = new Mock<ILogger<ApproveRequestCommandHandler>>();

        var handler = new ApproveRequestCommandHandler(
            approvalRepo.Object,
            workflowRepo.Object,
            auditRepo.Object,
            metrics.Object,
            publisher.Object,
            logger.Object);

        var command = new ApproveRequestCommand
        {
            ApprovalRequestId = Guid.NewGuid(),
            ApproverId = "approver-1",
            Comments = "ok"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ApprovalStatus.Approved, approval.Status);
        metrics.Verify(m => m.IncrementApprovalAction("approved"), Times.Once);
    }
}
