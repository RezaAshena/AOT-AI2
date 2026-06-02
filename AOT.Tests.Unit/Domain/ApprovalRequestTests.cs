using AOT.Domain.Aggregates;
using AOT.Domain.Enums;
using AOT.Domain.Events;

namespace AOT.Tests.Unit.Domain;

public class ApprovalRequestTests
{
    [Fact]
    public void Approve_WhenPending_SetsApprovedAndAddsDomainEvent()
    {
        var approval = new ApprovalRequest
        {
            Title = "Approval",
            Description = "desc",
            RequesterId = "requester-1",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        approval.Approve("approver-1", "ok");

        Assert.Equal(ApprovalStatus.Approved, approval.Status);
        Assert.Equal("approver-1", approval.ApproverId);
        Assert.Contains(approval.DomainEvents, e => e is ApprovalGrantedEvent);
    }
}
