namespace AOT.Workflows.Models;

public sealed record MarketResearchWorkflowInput(string Topic, string RequestedBy, bool RequiresApproval = true);

public sealed record MarketResearchSignal(string Branch, string Content);

public sealed record MarketResearchWorkflowOutput(
    string Topic,
    IReadOnlyList<MarketResearchSignal> Signals,
    bool Approved,
    string Summary,
    DateTime CompletedAtUtc);
