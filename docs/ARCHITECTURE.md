# Solution Structure

## Overview
This solution follows Clean Architecture principles with clear separation of concerns and dependency inversion.

```
AOT-AI/
├── AOT-AI.slnx                     # Solution file
│
├── docs/                            # Documentation
│   ├── REQUIREMENTS.md              # Requirements traceability matrix
│   ├── ARCHITECTURE.md              # Architecture diagrams and decisions
│   ├── AGENTS.md                    # Agent catalog and specifications
│   └── WORKFLOWS.md                 # Workflow orchestration documentation
│
├── src/
│   ├── AOT-UI/                      # Blazor Web App (Presentation Layer)
│   │   ├── Components/
│   │   │   ├── Layout/              # Layouts, NavMenu, MainLayout
│   │   │   ├── Pages/               # Routable pages (@page)
│   │   │   ├── Workflows/           # Workflow-specific components
│   │   │   ├── Agents/              # Agent execution panels
│   │   │   ├── Approvals/           # Approval inbox and details
│   │   │   └── Shared/              # Reusable UI components
│   │   ├── Services/                # UI-specific services (state management)
│   │   ├── wwwroot/                 # Static assets
│   │   ├── Program.cs               # Application entry point
│   │   └── appsettings.json         # Configuration
│   │
│   ├── AOT.Domain/                  # Domain Layer (Core Business Logic)
│   │   ├── Entities/                # Domain entities (WorkflowInstance, AgentExecution)
│   │   ├── ValueObjects/            # Immutable value objects
│   │   ├── Aggregates/              # Aggregate roots (ApprovalRequest)
│   │   ├── Events/                  # Domain events
│   │   ├── Enums/                   # Domain enumerations
│   │   ├── Exceptions/              # Domain-specific exceptions
│   │   └── Interfaces/              # Repository contracts (no implementation)
│   │
│   ├── AOT.Application/             # Application Layer (Use Cases)
│   │   ├── Commands/                # CQRS commands
│   │   ├── Queries/                 # CQRS queries
│   │   ├── Handlers/                # MediatR handlers for commands/queries
│   │   ├── DTOs/                    # Data transfer objects
│   │   ├── Validators/              # FluentValidation validators
│   │   ├── Behaviors/               # Pipeline behaviors (logging, validation)
│   │   ├── Services/                # Application service contracts
│   │   ├── Mappers/                 # Domain <-> DTO mapping
│   │   └── Security/                # Authorization policies
│   │
│   ├── AOT.Infrastructure/          # Infrastructure Layer (External Concerns)
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/      # EF Core entity configurations
│   │   │   ├── Migrations/          # EF Core migrations
│   │   │   └── Repositories/        # Repository implementations
│   │   ├── Identity/                # ASP.NET Core Identity setup
│   │   ├── Logging/                 # Serilog configuration
│   │   ├── Telemetry/               # OpenTelemetry setup
│   │   ├── Notifications/           # Notification service implementations
│   │   ├── Caching/                 # Distributed cache implementations
│   │   ├── ExternalServices/        # Third-party API integrations
│   │   └── DependencyInjection.cs   # Infrastructure service registration
│   │
│   ├── AOT.Agents/                  # Agent Framework Layer
│   │   ├── Core/
│   │   │   ├── IAgent.cs            # Agent interface
│   │   │   ├── AgentBase.cs         # Abstract base agent
│   │   │   └── AgentContext.cs      # Agent execution context
│   │   ├── Orchestration/
│   │   │   ├── AgentOrchestrator.cs # Multi-agent coordination
│   │   │   └── AgentRegistry.cs     # Agent discovery/factory
│   │   ├── MarketResearch/
│   │   │   ├── FashionMarketResearchAgent.cs
│   │   │   └── MarketResearchPromptTemplate.cs
│   │   ├── Analysis/                # Analytical agents
│   │   ├── Content/                 # Content generation agents
│   │   └── Validation/              # Validation agents
│   │
│   ├── AOT.Workflows/               # Workflow Orchestration Layer
│   │   ├── Core/
│   │   │   ├── IWorkflowContext.cs
│   │   │   ├── WorkflowContext.cs
│   │   │   └── IResettableExecutor.cs
│   │   ├── Executors/               # Typed executors (Executor<T>)
│   │   │   ├── InitializationExecutor.cs
│   │   │   ├── FanOutExecutor.cs
│   │   │   ├── AggregationExecutor.cs
│   │   │   └── TerminalExecutor.cs
│   │   ├── Factories/
│   │   │   └── WorkflowFactory.cs   # Graph construction
│   │   ├── Models/                  # Workflow-specific DTOs
│   │   └── StateManagement/         # Workflow state persistence
│   │
│   └── AOT.Shared/                  # Shared Kernel (Cross-cutting)
│       ├── Constants/               # Application-wide constants
│       ├── Extensions/              # Extension methods
│       ├── Helpers/                 # Utility helpers
│       └── Results/                 # Result/Either pattern types
│
└── tests/
	├── AOT.Tests.Unit/              # Unit Tests
	│   ├── Domain/                  # Domain logic tests
	│   ├── Application/             # Handler tests
	│   ├── Agents/                  # Agent tests (mocked dependencies)
	│   └── Workflows/               # Workflow executor logic tests
	│
	└── AOT.Tests.Integration/       # Integration Tests
		├── Persistence/             # Repository/DbContext tests
		├── Workflows/               # Full workflow execution tests
		├── API/                     # API endpoint tests
		└── Fixtures/                # Test fixtures and helpers
```

## Dependency Flow

```
AOT-UI (Presentation)
   ↓ depends on
AOT.Application, AOT.Infrastructure, AOT.Workflows, AOT.Shared
   ↓
AOT.Workflows
   ↓ depends on
AOT.Application, AOT.Agents
   ↓
AOT.Agents
   ↓ depends on
AOT.Application, AOT.Shared
   ↓
AOT.Infrastructure
   ↓ depends on
AOT.Application, AOT.Domain
   ↓
AOT.Application
   ↓ depends on
AOT.Domain, AOT.Shared
   ↓
AOT.Domain  ← NO DEPENDENCIES (Pure domain logic)
   ↕
AOT.Shared  ← Shared by all layers
```

## Key Architectural Decisions

### 1. Clean Architecture
- Domain layer has zero external dependencies
- Dependency Inversion: Interfaces in Application/Domain, implementations in Infrastructure/Agents/Workflows
- Use cases are orchestrated via MediatR handlers in Application layer

### 2. CQRS (Command Query Responsibility Segregation)
- Commands modify state (Commands/ folder)
- Queries retrieve state (Queries/ folder)
- Separate handlers for each
- Validated via pipeline behaviors

### 3. Repository Pattern
- Interfaces: `AOT.Domain/Interfaces/IRepository.cs`
- Implementations: `AOT.Infrastructure/Persistence/Repositories/`
- Unit of Work pattern via DbContext

### 4. Agent Framework
- Base abstractions in `AOT.Agents/Core/`
- Concrete agents implement `IAgent` or inherit from `AgentBase`
- Orchestrator coordinates multi-agent workflows
- Uses Microsoft.Agents.AI and Microsoft.Extensions.AI

### 5. Workflow Orchestration
- Microsoft.Agents.AI.Workflows with WorkflowBuilder
- Strongly typed executors (`Executor<T>`)
- Graph construction in `WorkflowFactory`
- Supports fan-out, fan-in, conditional routing, aggregation, reset

### 6. Blazor Frontend
- Interactive Server render mode
- Component-based UI in `AOT-UI/Components/`
- State management via scoped services
- Real-time updates for workflow/agent progress

### 7. Testing Strategy
- Unit tests: Domain logic, handlers, executors (fast, isolated)
- Integration tests: Persistence, workflows, APIs (slower, with dependencies)
- E2E tests: Critical user journeys (Playwright/Selenium optional)

### 8. Configuration Management
- appsettings.json for non-sensitive config
- Azure Key Vault / User Secrets for sensitive keys
- Options pattern throughout (`IOptions<T>`)

### 9. Observability
- Structured logging: Serilog with sinks (Console, File, Application Insights)
- Distributed tracing: OpenTelemetry
- Health checks: `/health` endpoint
- Metrics: Agent execution times, workflow durations, failure rates

### 10. Security
- Authentication: ASP.NET Core Identity (placeholder for Azure AD B2C)
- Authorization: Policy-based (defined in Application/Security/)
- Input validation: FluentValidation + Data Annotations
- Secure configuration: Key Vault integration
- Anti-forgery tokens for forms

## Next Steps
See `REQUIREMENTS.md` for detailed implementation roadmap and traceability.
