using AOT.Domain.Entities;
using AOT.Shared.Results;

namespace AOT.Application.Queries;

public record GetWorkflowByIdQuery(Guid WorkflowInstanceId) : IRequest<Result<WorkflowInstance>>;

public record GetAllWorkflowsQuery : IRequest<Result<IEnumerable<WorkflowInstance>>>;

public record GetPendingApprovalsQuery : IRequest<Result<IEnumerable<Domain.Aggregates.ApprovalRequest>>>;

public record GetAgentExecutionsByWorkflowQuery(Guid WorkflowInstanceId) : IRequest<Result<IEnumerable<AgentExecution>>>;
