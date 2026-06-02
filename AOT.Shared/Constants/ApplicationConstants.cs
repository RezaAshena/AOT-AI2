namespace AOT.Shared.Constants;

public static class ApplicationConstants
{
    public const string ApplicationName = "AOT Multi-Agent Platform";

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
        public const string Approver = "Approver";
    }

    public static class Policies
    {
        public const string RequireAdministrator = "RequireAdministrator";
        public const string RequireApprover = "RequireApprover";
    }

    public static class HealthChecks
    {
        public const string Database = "database";
        public const string AgentFramework = "agent_framework";
        public const string WorkflowEngine = "workflow_engine";
    }

    public static class CacheKeys
    {
        public const string WorkflowStatePrefix = "workflow:";
        public const string AgentResultPrefix = "agent:";
    }
}
