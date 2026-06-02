# Requirements Traceability Matrix

## Document Purpose
This document maps requirements from Assessment/Instructions.pdf and user instructions to design decisions and implementation artifacts.

## Source Documents
- **Primary**: `AOT-UI/wwwroot/Assessment/Instructions.pdf`
- **Secondary**: User instructions provided in task description
- **Tertiary**: `copilot-instructions.md` (fashion market research agent preferences)

---

## High-Level Requirements

### R1: Multi-Agent System
**Requirement**: Implement enterprise-grade multi-agent application using Microsoft Agentic Framework  
**Priority**: Critical  
**Design Element**: Agent orchestration layer with base abstractions  
**Artifacts**:
- `AOT.Agents/Core/IAgent.cs`
- `AOT.Agents/Core/AgentBase.cs`
- `AOT.Agents/Orchestration/AgentOrchestrator.cs`

### R2: Workflow Engine
**Requirement**: Implement workflow orchestration with fan-out, fan-in, conditional routing, aggregation, reset logic  
**Priority**: Critical  
**Design Element**: WorkflowBuilder-based graph construction  
**Artifacts**:
- `AOT.Workflows/WorkflowFactory.cs`
- `AOT.Workflows/Executors/{TypedExecutor}.cs`
- `AOT.Workflows/Core/WorkflowContext.cs`

### R3: Blazor Frontend
**Requirement**: Build UI using Blazor (.NET 10)  
**Priority**: Critical  
**Design Element**: Component-based architecture with state management  
**Artifacts**:
- `AOT-UI/Components/Pages/Dashboard.razor`
- `AOT-UI/Components/Workflows/WorkflowViewer.razor`
- `AOT-UI/Components/Agents/AgentExecutionPanel.razor`

### R4: Clean Architecture
**Requirement**: Enterprise-level architecture with separation of concerns  
**Priority**: Critical  
**Design Element**: Layered solution (Domain, Application, Infrastructure, Presentation)  
**Artifacts**:
- `AOT.Domain/` project
- `AOT.Application/` project
- `AOT.Infrastructure/` project
- Project reference topology enforces dependency direction

### R5: CQRS + Mediator Pattern
**Requirement**: Command/Query segregation with mediator orchestration  
**Priority**: High  
**Design Element**: MediatR-based handlers with validation pipeline  
**Artifacts**:
- `AOT.Application/Commands/{Command}.cs`
- `AOT.Application/Queries/{Query}.cs`
- `AOT.Application/Handlers/{Handler}.cs`

### R6: Approval System
**Requirement**: Human-in-the-loop approval workflow with audit trail  
**Priority**: High  
**Design Element**: Approval aggregate with state machine  
**Artifacts**:
- `AOT.Domain/Aggregates/ApprovalRequest.cs`
- `AOT.Application/Services/IApprovalService.cs`
- `AOT-UI/Components/Approvals/ApprovalInbox.razor`

### R7: Notification System
**Requirement**: Event-driven notifications (in-app + extensible channels)  
**Priority**: Medium  
**Design Element**: Domain events trigger notification handlers  
**Artifacts**:
- `AOT.Domain/Events/{DomainEvent}.cs`
- `AOT.Application/Handlers/NotificationHandler.cs`
- `AOT.Infrastructure/Notifications/NotificationService.cs`

### R8: Security
**Requirement**: Authentication, authorization, secure configuration, input validation  
**Priority**: Critical  
**Design Element**: ASP.NET Core Identity + policy-based authorization  
**Artifacts**:
- `AOT.Infrastructure/Identity/IdentityConfiguration.cs`
- `AOT.Application/Security/Policies.cs`
- Secure appsettings (Azure Key Vault integration)

### R9: Observability
**Requirement**: Logging, monitoring, metrics, distributed tracing  
**Priority**: High  
**Design Element**: Structured logging (Serilog) + OpenTelemetry  
**Artifacts**:
- `AOT.Infrastructure/Logging/LoggingConfiguration.cs`
- `AOT.Infrastructure/Telemetry/TelemetryConfiguration.cs`
- Health check endpoints

### R10: Persistence
**Requirement**: Workflow state, agent executions, audit logs  
**Priority**: Critical  
**Design Element**: EF Core with repository pattern  
**Artifacts**:
- `AOT.Infrastructure/Persistence/ApplicationDbContext.cs`
- `AOT.Infrastructure/Repositories/{Repository}.cs`
- `AOT.Domain/Entities/{Entity}.cs`

---

## Fashion Market Research Agent (from copilot-instructions.md)

### R11: Constrained Market Research Prompt
**Requirement**: Specific prompt style with strict word limit, bullet points, fixed sections (Trend Fit, Target Market, Competition, Demand, Price)  
**Priority**: Medium  
**Design Element**: Specialized agent with structured output format  
**Artifacts**:
- `AOT.Agents/MarketResearch/FashionMarketResearchAgent.cs`
- `AOT.Agents/MarketResearch/MarketResearchPromptTemplate.cs`

---

## Workflow Patterns (from user instructions)

### R12: Fan-Out Pattern
**Requirement**: Execute multiple agents concurrently  
**Design Element**: `AddFanOutEdge` in WorkflowBuilder  
**Implementation**: Parallel task initiation at workflow graph branch points

### R13: Fan-In Pattern
**Requirement**: Aggregate results from parallel executions  
**Design Element**: `AddFanInBarrierEdge` with aggregator executor  
**Implementation**: Wait for all parallel branches, then merge results

### R14: Conditional Routing
**Requirement**: Dynamic workflow path based on runtime state  
**Design Element**: Conditional edge predicates  
**Implementation**: Route to different executors based on workflow context

### R15: State Reset Logic
**Requirement**: Allow workflow to reset and re-execute  
**Design Element**: `IResettableExecutor` interface  
**Implementation**: Executors implement reset method to clear intermediate state

### R16: Workflow Output
**Requirement**: Yield structured output from workflow execution  
**Design Element**: `YieldOutputAsync` in executors  
**Implementation**: Terminal executors yield final workflow results

---

## Non-Functional Requirements

### NFR1: Scalability
- Async/await throughout
- Stateless agent design where possible
- Connection pooling, caching

### NFR2: Maintainability
- SOLID principles
- Clear naming conventions
- Minimal comments (self-documenting code)
- XML documentation for public APIs

### NFR3: Testability
- Unit tests for domain/application logic
- Integration tests for workflows/persistence
- E2E tests for critical UI flows

### NFR4: Performance
- Response time < 2s for UI interactions
- Workflow execution monitored with metrics
- Database query optimization (indexes)

### NFR5: Reliability
- Retry policies for transient failures
- Circuit breaker for external dependencies
- Compensating transactions for workflow failures

---

## Assumption Log

1. **Database**: Using SQL Server (or compatible) with EF Core migrations.
2. **Authentication**: Placeholder for future Azure AD B2C or similar.
3. **AI Models**: Using Azure OpenAI or compatible endpoint via `Microsoft.Extensions.AI`.
4. **Deployment**: Docker containerization, Azure App Service or AKS.
5. **Message Bus**: In-memory domain events initially; can upgrade to Azure Service Bus.
6. **File Storage**: Local file system initially; can migrate to Azure Blob Storage.

---

## Traceability Table

| Requirement ID | Design Element | Code Artifact | Test Coverage |
|----------------|----------------|---------------|---------------|
| R1 | Agent Base | `AOT.Agents/Core/AgentBase.cs` | `AOT.Tests.Unit/Agents/AgentBaseTests.cs` |
| R2 | Workflow Factory | `AOT.Workflows/WorkflowFactory.cs` | `AOT.Tests.Integration/Workflows/WorkflowExecutionTests.cs` |
| R3 | Blazor Components | `AOT-UI/Components/**/*.razor` | `AOT.Tests.E2E/UITests.cs` |
| R4 | Clean Architecture | Project structure | Architecture tests |
| R5 | CQRS Handlers | `AOT.Application/Handlers/` | `AOT.Tests.Unit/Handlers/` |
| R6 | Approval Aggregate | `AOT.Domain/Aggregates/ApprovalRequest.cs` | `AOT.Tests.Unit/Domain/ApprovalTests.cs` |
| R7 | Notification Service | `AOT.Infrastructure/Notifications/` | `AOT.Tests.Integration/NotificationTests.cs` |
| R8 | Security Policies | `AOT.Application/Security/` | Security tests |
| R9 | Telemetry | `AOT.Infrastructure/Telemetry/` | Observability tests |
| R10 | EF Core Persistence | `AOT.Infrastructure/Persistence/` | `AOT.Tests.Integration/RepositoryTests.cs` |
| R11 | Fashion Agent | `AOT.Agents/MarketResearch/` | `AOT.Tests.Unit/Agents/FashionAgentTests.cs` |

---

## Phase Mapping

- **Phase 1 (Foundation)**: R4, R8, R9, NFR1-5
- **Phase 2 (Agent Framework)**: R1, R11
- **Phase 3 (Workflow Engine)**: R2, R12-16
- **Phase 4 (UI Integration)**: R3
- **Phase 5 (Approval System)**: R6
- **Phase 6 (Notifications)**: R7
- **Phase 7 (Testing & Deployment)**: All test artifacts, CI/CD configuration

---

**Note**: Detailed requirements from `Instructions.pdf` will be incorporated once the PDF is parsed. This document will be updated as implementation progresses.
