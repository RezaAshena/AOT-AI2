using AOT.Agents.Core;
using AOT.Agents.MarketResearch;
using AOT.Agents.Orchestration;
using Microsoft.Extensions.DependencyInjection;

namespace AOT.Agents;

public static class DependencyInjection
{
    public static IServiceCollection AddAgents(this IServiceCollection services)
    {
        services.AddScoped<IAgent, FashionMarketResearchAgent>();
        services.AddScoped<IAgentRegistry, AgentRegistry>();
        services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

        return services;
    }
}
