using AOT.Agents.Core;

namespace AOT.Agents.Orchestration;

public class AgentRegistry : IAgentRegistry
{
    private readonly IReadOnlyDictionary<string, IAgent> _agents;

    public AgentRegistry(IEnumerable<IAgent> agents)
    {
        _agents = agents.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
    }

    public IAgent Resolve(string agentName)
    {
        if (_agents.TryGetValue(agentName, out var agent))
        {
            return agent;
        }

        throw new KeyNotFoundException($"Agent '{agentName}' is not registered.");
    }

    public IReadOnlyCollection<IAgent> GetAll() => _agents.Values.ToArray();
}
