using AOT.Domain.Aggregates;
using AOT.Domain.Interfaces;
using AOT.Infrastructure.Persistence;
using AOT.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AOT.Tests.Integration.Persistence;

public class ApprovalRepositoryIntegrationTests
{
    [Fact]
    public async Task GetPendingAsync_ReturnsOnlyPendingRequests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"approval-test-{Guid.NewGuid()}")
            .Options;

        await using var db = new ApplicationDbContext(options);
        IApprovalRequestRepository repository = new ApprovalRequestRepository(db);

        var pending = new ApprovalRequest
        {
            Title = "Pending",
            Description = "desc",
            RequesterId = "req-1",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var approved = new ApprovalRequest
        {
            Title = "Approved",
            Description = "desc",
            RequesterId = "req-2",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        approved.Approve("approver-1", "ok");

        await repository.AddAsync(pending);
        await repository.AddAsync(approved);
        await repository.SaveChangesAsync();

        var results = (await repository.GetPendingAsync()).ToList();

        Assert.Single(results);
        Assert.Equal("Pending", results[0].Title);
    }
}
