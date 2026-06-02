using AOT.Domain.Entities;
using AOT.Domain.Enums;

namespace AOT.Tests.Unit.Domain;

public class WorkflowInstanceTests
{
    [Fact]
    public void Start_WhenNotStarted_TransitionsToRunning()
    {
        var workflow = new WorkflowInstance
        {
            WorkflowName = "test",
            WorkflowDefinitionId = "def-1"
        };

        workflow.Start();

        Assert.Equal(WorkflowStatus.Running, workflow.Status);
        Assert.NotNull(workflow.StartedAt);
    }

    [Fact]
    public void Complete_WhenNotRunning_ThrowsInvalidOperationException()
    {
        var workflow = new WorkflowInstance
        {
            WorkflowName = "test",
            WorkflowDefinitionId = "def-1"
        };

        Assert.Throws<InvalidOperationException>(() => workflow.Complete());
    }
}
