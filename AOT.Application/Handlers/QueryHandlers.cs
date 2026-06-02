using AOT.Application.Queries;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class GetWorkflowByIdQueryHandler : IRequestHandler<GetWorkflowByIdQuery, Result<WorkflowInstance>>
{
    private readonly IWorkflowRepository _repository;
    private readonly ILogger<GetWorkflowByIdQueryHandler> _logger;

    public GetWorkflowByIdQueryHandler(
        IWorkflowRepository repository,
        ILogger<GetWorkflowByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<WorkflowInstance>> Handle(GetWorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var workflow = await _repository.GetByIdAsync(request.WorkflowInstanceId, cancellationToken);
            return workflow == null 
                ? Result.Failure<WorkflowInstance>("Workflow not found")
                : Result.Success(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get workflow {WorkflowId}", request.WorkflowInstanceId);
            return Result.Failure<WorkflowInstance>($"Failed to get workflow: {ex.Message}");
        }
    }
}

public class GetAllWorkflowsQueryHandler : IRequestHandler<GetAllWorkflowsQuery, Result<IEnumerable<WorkflowInstance>>>
{
    private readonly IWorkflowRepository _repository;
    private readonly ILogger<GetAllWorkflowsQueryHandler> _logger;

    public GetAllWorkflowsQueryHandler(
        IWorkflowRepository repository,
        ILogger<GetAllWorkflowsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<WorkflowInstance>>> Handle(GetAllWorkflowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var workflows = await _repository.GetAllAsync(cancellationToken);
            return Result.Success(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all workflows");
            return Result.Failure<IEnumerable<WorkflowInstance>>($"Failed to get workflows: {ex.Message}");
        }
    }
}

public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, Result<IEnumerable<Domain.Aggregates.ApprovalRequest>>>
{
    private readonly IApprovalRequestRepository _repository;
    private readonly ILogger<GetPendingApprovalsQueryHandler> _logger;

    public GetPendingApprovalsQueryHandler(
        IApprovalRequestRepository repository,
        ILogger<GetPendingApprovalsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<Domain.Aggregates.ApprovalRequest>>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var approvals = await _repository.GetPendingAsync(cancellationToken);
            return Result.Success(approvals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pending approvals");
            return Result.Failure<IEnumerable<Domain.Aggregates.ApprovalRequest>>($"Failed to get pending approvals: {ex.Message}");
        }
    }
}
