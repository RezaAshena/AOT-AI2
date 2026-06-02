using AOT.Application.Commands;
using AOT.Domain.Entities;
using AOT.Domain.Interfaces;
using AOT.Shared.Results;

namespace AOT.Application.Handlers;

public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, Result<Guid>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ILogger<CreateWorkflowCommandHandler> _logger;

    public CreateWorkflowCommandHandler(
        IWorkflowRepository workflowRepository,
        ILogger<CreateWorkflowCommandHandler> logger)
    {
        _workflowRepository = workflowRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var workflow = new WorkflowInstance
            {
                WorkflowName = request.WorkflowName,
                WorkflowDefinitionId = request.WorkflowDefinitionId
            };

            workflow.SetContext(request.InitialContext);

            await _workflowRepository.AddAsync(workflow, cancellationToken);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created workflow {WorkflowId} of type {WorkflowName}", 
                workflow.Id, workflow.WorkflowName);

            return Result.Success(workflow.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create workflow {WorkflowName}", request.WorkflowName);
            return Result.Failure<Guid>($"Failed to create workflow: {ex.Message}");
        }
    }
}

public class StartWorkflowCommandHandler : IRequestHandler<StartWorkflowCommand, Result>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ILogger<StartWorkflowCommandHandler> _logger;

    public StartWorkflowCommandHandler(
        IWorkflowRepository workflowRepository,
        ILogger<StartWorkflowCommandHandler> logger)
    {
        _workflowRepository = workflowRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(StartWorkflowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowInstanceId, cancellationToken);
            if (workflow == null)
                return Result.Failure("Workflow not found");

            workflow.Start();
            await _workflowRepository.UpdateAsync(workflow, cancellationToken);
            await _workflowRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Started workflow {WorkflowId}", workflow.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start workflow {WorkflowId}", request.WorkflowInstanceId);
            return Result.Failure($"Failed to start workflow: {ex.Message}");
        }
    }
}
