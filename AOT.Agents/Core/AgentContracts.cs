namespace AOT.Agents.Core;

public sealed record AgentExecutionRequest(
    string Input,
    string? CorrelationId = null,
    IReadOnlyDictionary<string, object>? Metadata = null);

public sealed record AgentExecutionResponse(
    bool Success,
    string Output,
    string? Error,
    TimeSpan Duration,
    int TokensUsed = 0);

public interface IAgent
{
    string Name { get; }
    string Category { get; }
    Task<AgentExecutionResponse> ExecuteAsync(AgentExecutionRequest request, CancellationToken cancellationToken = default);
}

public interface IAgentRegistry
{
    IAgent Resolve(string agentName);
    IReadOnlyCollection<IAgent> GetAll();
}

public interface IAgentOrchestrator
{
    Task<IReadOnlyCollection<AgentExecutionResponse>> ExecuteConcurrentAsync(
        IEnumerable<string> agentNames,
        AgentExecutionRequest request,
        CancellationToken cancellationToken = default);
}
