using Microsoft.Extensions.Logging;

namespace AOT.Agents.Core;

public abstract class AgentBase : IAgent
{
    private readonly ILogger _logger;

    protected AgentBase(ILogger logger)
    {
        _logger = logger;
    }

    public abstract string Name { get; }
    public abstract string Category { get; }

    protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(45);
    protected virtual int MaxRetries => 2;

    public async Task<AgentExecutionResponse> ExecuteAsync(AgentExecutionRequest request, CancellationToken cancellationToken = default)
    {
        var startedAt = DateTime.UtcNow;
        Exception? lastException = null;

        for (var attempt = 1; attempt <= MaxRetries + 1; attempt++)
        {
            using var timeoutCts = new CancellationTokenSource(Timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                _logger.LogInformation("Executing agent {AgentName} attempt {Attempt}/{TotalAttempts}", Name, attempt, MaxRetries + 1);

                var output = await ExecuteCoreAsync(request, linkedCts.Token);
                var elapsed = DateTime.UtcNow - startedAt;

                return new AgentExecutionResponse(
                    Success: true,
                    Output: output,
                    Error: null,
                    Duration: elapsed,
                    TokensUsed: EstimateTokens(output));
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                lastException = new TimeoutException($"Agent {Name} timed out after {Timeout.TotalSeconds} seconds.");
                _logger.LogWarning(lastException, "Timeout in agent {AgentName} attempt {Attempt}", Name, attempt);
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning(ex, "Agent {AgentName} failed on attempt {Attempt}", Name, attempt);
            }

            if (attempt <= MaxRetries)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken);
            }
        }

        var failedElapsed = DateTime.UtcNow - startedAt;
        return new AgentExecutionResponse(
            Success: false,
            Output: string.Empty,
            Error: lastException?.Message ?? "Unknown error",
            Duration: failedElapsed,
            TokensUsed: 0);
    }

    protected abstract Task<string> ExecuteCoreAsync(AgentExecutionRequest request, CancellationToken cancellationToken);

    protected virtual int EstimateTokens(string text) => Math.Max(1, text.Length / 4);
}
