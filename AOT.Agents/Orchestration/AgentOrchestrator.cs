using AOT.Agents.Core;
using Microsoft.Extensions.Logging;

namespace AOT.Agents.Orchestration;

public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly IAgentRegistry _agentRegistry;
    private readonly ILogger<AgentOrchestrator> _logger;

    public AgentOrchestrator(IAgentRegistry agentRegistry, ILogger<AgentOrchestrator> logger)
    {
        _agentRegistry = agentRegistry;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<AgentExecutionResponse>> ExecuteConcurrentAsync(
        IEnumerable<string> agentNames,
        AgentExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        var agents = agentNames.Select(_agentRegistry.Resolve).ToArray();
        _logger.LogInformation("Running {Count} agents concurrently", agents.Length);

        var tasks = agents.Select(agent => agent.ExecuteAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);

        return results;
    }
}
