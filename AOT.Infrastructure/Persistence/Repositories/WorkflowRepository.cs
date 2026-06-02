using AOT.Domain.Entities;
using AOT.Domain.Enums;
using AOT.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AOT.Infrastructure.Persistence.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WorkflowRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkflowInstances
            .Include(w => w.Steps)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkflowInstances
            .Include(w => w.Steps)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(WorkflowStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkflowInstances
            .Include(w => w.Steps)
            .Where(w => w.Status == status)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkflowInstances.AddAsync(workflow, cancellationToken);
    }

    public Task UpdateAsync(WorkflowInstance workflow, CancellationToken cancellationToken = default)
    {
        _dbContext.WorkflowInstances.Update(workflow);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
