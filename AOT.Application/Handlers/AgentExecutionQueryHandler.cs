using AOT.Application.Queries;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class GetAgentExecutionsByWorkflowQueryHandler : IRequestHandler<GetAgentExecutionsByWorkflowQuery, Result<IEnumerable<AgentExecution>>>
{
    private readonly IAgentExecutionRepository _repository;
    private readonly ILogger<GetAgentExecutionsByWorkflowQueryHandler> _logger;

    public GetAgentExecutionsByWorkflowQueryHandler(IAgentExecutionRepository repository, ILogger<GetAgentExecutionsByWorkflowQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<AgentExecution>>> Handle(GetAgentExecutionsByWorkflowQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var items = await _repository.GetByWorkflowInstanceIdAsync(request.WorkflowInstanceId, cancellationToken);
            return Result.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get agent executions for workflow {WorkflowInstanceId}", request.WorkflowInstanceId);
            return Result.Failure<IEnumerable<AgentExecution>>($"Failed to get agent executions: {ex.Message}");
        }
    }
}
