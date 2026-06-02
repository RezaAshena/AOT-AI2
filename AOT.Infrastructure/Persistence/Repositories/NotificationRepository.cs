using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AOT.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NotificationMessage?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<NotificationMessage>()
            .FirstOrDefaultAsync(n => n.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<IEnumerable<NotificationMessage>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<NotificationMessage>()
            .Where(n => !n.IsSent)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<NotificationMessage>().AddAsync(notification, cancellationToken);
    }

    public Task UpdateAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<NotificationMessage>().Update(notification);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
