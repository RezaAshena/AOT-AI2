# Deployment & Readiness

## Environment
- .NET 10
- Blazor Web App (`AOT-UI`)
- SQL Server connection via `DefaultConnection`

## Run Locally
1. Restore/build:
   - `dotnet build AOT-AI.slnx`
2. Tests:
   - `dotnet test AOT.Tests.Unit/AOT.Tests.Unit.csproj`
   - `dotnet test AOT.Tests.Integration/AOT.Tests.Integration.csproj`
3. Start app:
   - `dotnet run --project AOT-UI/AOT-UI.csproj`

## Database
- EF Core context: `ApplicationDbContext`
- If migration generation was interrupted in-session, run:
  - `dotnet ef migrations add <Name> --project AOT.Infrastructure/AOT.Infrastructure.csproj --startup-project AOT-UI/AOT-UI.csproj --output-dir Persistence/Migrations`
  - `dotnet ef database update --project AOT.Infrastructure/AOT.Infrastructure.csproj --startup-project AOT-UI/AOT-UI.csproj`

## Operational Endpoints
- Health: `/health`
- Approval APIs:
  - `GET /api/approvals/pending`
  - `POST /api/approvals/create`
  - `POST /api/approvals/{id}/approve`
  - `POST /api/approvals/{id}/reject`

## Security & Reliability Checklist
- HTTPS + HSTS
- Antiforgery enabled
- Security headers middleware enabled
- Correlation ID middleware enabled
- Centralized exception handling enabled
- Structured Serilog logging enabled

## Acceptance Evidence (current workspace)
- Solution build: passing
- Unit test project: passing
- Integration test project: passing
- Blazor dashboard/pages for workflows/approvals/monitoring: implemented
