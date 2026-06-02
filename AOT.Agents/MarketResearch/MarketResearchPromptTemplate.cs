namespace AOT.Agents.MarketResearch;

public static class MarketResearchPromptTemplate
{
    public static string Build(string topic)
    {
        return $"""
        Produce concise fashion market research for: {topic}

        Constraints:
        - Strict word limit: 150 words total.
        - Use bullet points only.
        - Direct output, no intro or outro.
        - Use exactly these sections in order:
          1) Trend Fit
          2) Target Market
          3) Competition
          4) Demand
          5) Price
        """;
    }
}
