using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AOT.Domain.Interfaces;
using AOT.Application.Services;
using Serilog;
using AOT.Infrastructure.Persistence;
using AOT.Infrastructure.Notifications;
using AOT.Infrastructure.Persistence.Repositories;
using AOT.Infrastructure.Telemetry;

namespace AOT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=AOT-AI;Trusted_Connection=True;MultipleActiveResultSets=true",
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database");

        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IAgentExecutionRepository, AgentExecutionRepository>();
        services.AddScoped<IApprovalRequestRepository, ApprovalRequestRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<OperationalMetrics>();
        services.AddSingleton<AOT.Application.Services.IOperationalMetrics>(sp => sp.GetRequiredService<OperationalMetrics>());

        ConfigureLogging(configuration);

        return services;
    }

    private static void ConfigureLogging(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "AOT-AI")
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/aot-ai-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();
    }
}
