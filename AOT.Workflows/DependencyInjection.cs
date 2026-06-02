using Microsoft.Extensions.DependencyInjection;

namespace AOT.Workflows;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflows(this IServiceCollection services)
    {
        return services;
    }
}
