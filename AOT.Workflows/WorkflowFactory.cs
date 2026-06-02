using AOT.Agents.Core;
using AOT.Agents.Orchestration;
using AOT.Workflows.Core;
using AOT.Workflows.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AOT.Workflows;

internal static class WorkflowFactory
{
    internal static Workflow BuildWorkflow(IChatClient chatClient)
    {
        // Create executors and agents
        var resetCoordinator = new WorkflowResetCoordinator();

        var start = ((MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            return ValueTask.FromResult(input);
        }).BindAsExecutor("start");

        var trend = (async (MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            await context.QueueStateUpdateAsync("topic", input.Topic, ct);
            return new MarketResearchSignal("Trend Fit", $"{input.Topic} aligns with current social-commerce and seasonal demand patterns.");
        }).BindAsExecutor("trend-fit");

        var targetMarket = ((MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            return ValueTask.FromResult(new MarketResearchSignal("Target Market", "Gen Z and young millennials seeking expressive and accessible fashion."));
        }).BindAsExecutor("target-market");

        var competition = ((MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            return ValueTask.FromResult(new MarketResearchSignal("Competition", "Strong DTC and fast-fashion competition; differentiation requires quality + identity."));
        }).BindAsExecutor("competition");

        var demand = ((MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            return ValueTask.FromResult(new MarketResearchSignal("Demand", "Demand is moderate-to-high in social and creator-led channels."));
        }).BindAsExecutor("demand");

        var price = ((MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            return ValueTask.FromResult(new MarketResearchSignal("Price", "Recommended pricing: accessible mid-tier with premium upsell variants."));
        }).BindAsExecutor("price");

        var aggregate = ((List<MarketResearchSignal>? current, MarketResearchSignal signal) =>
        {
            current ??= new List<MarketResearchSignal>();
            current.Add(signal);
            return current;
        }).BindAsExecutor<MarketResearchSignal, List<MarketResearchSignal>>("aggregate");

        var approval = ((List<MarketResearchSignal> signals, IWorkflowContext context, CancellationToken ct) =>
        {
            // Approval flow hook: can be replaced with human-in-the-loop request/response.
            var approved = signals.Count >= 5;
            return ValueTask.FromResult(approved);
        }).BindAsExecutor("approval");

        var approvedOutput = (async (bool approved, IWorkflowContext context, CancellationToken ct) =>
        {
            var topic = await context.GetStateEntryAsync<string>("topic", ct) ?? "Unknown";
            var signals = await context.GetStateEntryAsync<List<MarketResearchSignal>>("aggregate", ct) ?? new List<MarketResearchSignal>();
            var output = new MarketResearchWorkflowOutput(
                Topic: topic,
                Signals: signals,
                Approved: approved,
                Summary: "Workflow approved and completed successfully.",
                CompletedAtUtc: DateTime.UtcNow);

            await context.YieldOutputAsync(output, ct);
            return output;
        }).BindAsExecutor("approved-output");

        var rejectedOutput = (async (bool approved, IWorkflowContext context, CancellationToken ct) =>
        {
            var topic = await context.GetStateEntryAsync<string>("topic", ct) ?? "Unknown";
            var signals = await context.GetStateEntryAsync<List<MarketResearchSignal>>("aggregate", ct) ?? new List<MarketResearchSignal>();
            var output = new MarketResearchWorkflowOutput(
                Topic: topic,
                Signals: signals,
                Approved: approved,
                Summary: "Workflow requires manual review before publication.",
                CompletedAtUtc: DateTime.UtcNow);

            await context.YieldOutputAsync(output, ct);
            return output;
        }).BindAsExecutor("rejected-output");

        var reset = (async (MarketResearchWorkflowInput input, IWorkflowContext context, CancellationToken ct) =>
        {
            await resetCoordinator.ResetAsync();
            await context.QueueClearScopeAsync(ct);
            await context.QueueStateUpdateAsync("topic", input.Topic, ct);
            return input;
        }).BindAsExecutor("reset");

        // Build workflow graph
        var builder = new WorkflowBuilder(start)
            .WithName("Fashion Market Research Workflow")
            .WithDescription("Multi-agent market research workflow with fan-out/fan-in, conditional routing, aggregation and reset.")
            .WithOutputFrom(approvedOutput, rejectedOutput);

        // Connect nodes using:
        // - AddEdge
        builder
            .AddEdge(reset, start)
            .AddEdge<List<MarketResearchSignal>>(aggregate, approval)
            .AddEdge<bool>(approval, approvedOutput, isApproved => isApproved == true, label: "approved")
            .AddEdge<bool>(approval, rejectedOutput, isApproved => isApproved == false, label: "rejected");

        // - AddFanOutEdge
        var parallelTargets = new[] { trend, targetMarket, competition, demand, price };
        builder.AddFanOutEdge(start, parallelTargets);

        // - AddFanInBarrierEdge
        builder.AddFanInBarrierEdge(parallelTargets, aggregate);

        // Return built workflow
        return builder.Build();
    }
}
