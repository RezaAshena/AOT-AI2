using AOT_UI.Components;
using AOT_UI.Middleware;
using AOT.Agents;
using AOT.Application;
using AOT.Application.Commands;
using AOT.Application.Queries;
using AOT.Domain.Aggregates;
using AOT.Domain.Interfaces;
using AOT.Shared.Constants;
using AOT.Infrastructure;
using AOT_UI.Services;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ApplicationConstants.Policies.RequireAdministrator,
        policy => policy.RequireRole(ApplicationConstants.Roles.Administrator));

    options.AddPolicy(ApplicationConstants.Policies.RequireApprover,
        policy => policy.RequireRole(ApplicationConstants.Roles.Administrator, ApplicationConstants.Roles.Approver));
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAgents();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<DashboardState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHealthChecks("/health");

var approvals = app.MapGroup("/api/approvals");

approvals.MapGet("/pending", async (IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(new GetPendingApprovalsQuery(), ct);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(new { error = result.Error });
});

approvals.MapPost("/create", async (CreateApprovalRequestRequest request, IApprovalRequestRepository repo, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.RequesterId))
    {
        return Results.BadRequest(new { error = "Title and RequesterId are required." });
    }

    if (request.ExpiresAtUtc <= DateTime.UtcNow)
    {
        return Results.BadRequest(new { error = "ExpiresAtUtc must be in the future." });
    }

    var approval = new ApprovalRequest
    {
        Title = request.Title,
        Description = request.Description,
        RequesterId = request.RequesterId,
        WorkflowInstanceId = request.WorkflowInstanceId,
        ExpiresAt = request.ExpiresAtUtc
    };

    await repo.AddAsync(approval, ct);
    await repo.SaveChangesAsync(ct);
    return Results.Ok(new { approvalRequestId = approval.Id });
});

approvals.MapPost("/{id:guid}/approve", async (Guid id, ApproveRequestDto request, IMediator mediator, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.ApproverId))
    {
        return Results.BadRequest(new { error = "ApproverId is required." });
    }

    var result = await mediator.Send(new ApproveRequestCommand
    {
        ApprovalRequestId = id,
        ApproverId = request.ApproverId,
        Comments = request.Comments
    }, ct);

    return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { error = result.Error });
});

approvals.MapPost("/{id:guid}/reject", async (Guid id, RejectRequestDto request, IMediator mediator, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.ApproverId))
    {
        return Results.BadRequest(new { error = "ApproverId is required." });
    }

    var result = await mediator.Send(new RejectRequestCommand
    {
        ApprovalRequestId = id,
        ApproverId = request.ApproverId,
        Comments = request.Comments
    }, ct);

    return result.IsSuccess ? Results.Ok() : Results.BadRequest(new { error = result.Error });
});

try
{
    Log.Information("Starting AOT Multi-Agent Platform");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

internal sealed record CreateApprovalRequestRequest(
    string Title,
    string Description,
    string RequesterId,
    Guid? WorkflowInstanceId,
    DateTime ExpiresAtUtc);

internal sealed record ApproveRequestDto(string ApproverId, string? Comments);

internal sealed record RejectRequestDto(string ApproverId, string? Comments);

