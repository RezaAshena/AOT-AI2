using Microsoft.Agents.AI.Workflows;

namespace AOT.Workflows.Core;

public sealed class WorkflowResetCoordinator : IResettableExecutor
{
    public bool WasReset { get; private set; }

    public ValueTask ResetAsync()
    {
        WasReset = true;
        return ValueTask.CompletedTask;
    }
}
