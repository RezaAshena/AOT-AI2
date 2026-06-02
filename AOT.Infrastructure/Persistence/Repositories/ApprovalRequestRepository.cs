using AOT.Domain.Aggregates;
using AOT.Domain.Enums;
using AOT.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AOT.Infrastructure.Persistence.Repositories;

public class ApprovalRequestRepository : IApprovalRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ApprovalRequestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApprovalRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApprovalRequests.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRequest>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApprovalRequests
            .Where(a => a.Status == ApprovalStatus.Pending)
            .OrderBy(a => a.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRequest>> GetByRequesterIdAsync(string requesterId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApprovalRequests
            .Where(a => a.RequesterId == requesterId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ApprovalRequest approvalRequest, CancellationToken cancellationToken = default)
    {
        await _dbContext.ApprovalRequests.AddAsync(approvalRequest, cancellationToken);
    }

    public Task UpdateAsync(ApprovalRequest approvalRequest, CancellationToken cancellationToken = default)
    {
        _dbContext.ApprovalRequests.Update(approvalRequest);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
