using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AOT.Infrastructure.Persistence.Repositories;

public class AgentExecutionRepository : IAgentExecutionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AgentExecutionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AgentExecution?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AgentExecutions.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AgentExecution>> GetByWorkflowInstanceIdAsync(Guid workflowInstanceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AgentExecutions
            .Where(a => a.WorkflowInstanceId == workflowInstanceId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AgentExecution agentExecution, CancellationToken cancellationToken = default)
    {
        await _dbContext.AgentExecutions.AddAsync(agentExecution, cancellationToken);
    }

    public Task UpdateAsync(AgentExecution agentExecution, CancellationToken cancellationToken = default)
    {
        _dbContext.AgentExecutions.Update(agentExecution);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
