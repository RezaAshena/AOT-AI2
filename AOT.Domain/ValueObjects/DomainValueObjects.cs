namespace AOT.Domain.ValueObjects;

public record AgentPrompt
{
    public required string Template { get; init; }
    public required Dictionary<string, string> Variables { get; init; }

    public string Render()
    {
        var result = Template;
        foreach (var (key, value) in Variables)
        {
            result = result.Replace($"{{{key}}}", value);
        }
        return result;
    }

    public static AgentPrompt Create(string template, Dictionary<string, string> variables)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template cannot be empty", nameof(template));

        return new AgentPrompt { Template = template, Variables = variables };
    }
}

public record WorkflowConfiguration
{
    public required string Name { get; init; }
    public required string Version { get; init; }
    public Dictionary<string, object> Settings { get; init; } = new();
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(30);
    public int MaxRetries { get; init; } = 3;
    public bool RequiresApproval { get; init; }
}
