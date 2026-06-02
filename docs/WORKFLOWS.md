# Workflow Orchestration Design

## Workflow Factory
- File: `AOT.Workflows/WorkflowFactory.cs`
- Entry point: `BuildWorkflow(IChatClient chatClient)`

## Execution Graph
1. **start** receives `MarketResearchWorkflowInput`
2. **fan-out branches** execute in parallel:
   - trend-fit
   - target-market
   - competition
   - demand
   - price
3. **fan-in barrier** joins branches into **aggregate**
4. **approval** evaluates aggregate state
5. **conditional routing**:
   - approved-output when true
   - rejected-output when false
6. **reset** node clears state and re-seeds topic context

## Required Patterns Implemented
- `AddEdge`
- `AddFanOutEdge`
- `AddFanInBarrierEdge`
- Conditional branch edges with typed predicates
- Aggregation with typed accumulator
- State read/write via `IWorkflowContext`
- Output emission via `YieldOutputAsync`
- Reset logic using `IResettableExecutor` coordinator

## Runtime State Strategy
- State keys stored in workflow context (e.g., `topic`, `aggregate`)
- Reset path uses `QueueClearScopeAsync`
- Output model: `MarketResearchWorkflowOutput`

## Error Handling
- Domain/application handlers return `Result`/`Result<T>`
- Global exception handler in presentation layer
- Notification and agent retries with bounded attempts
