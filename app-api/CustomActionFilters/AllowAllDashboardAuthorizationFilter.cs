using Hangfire.Annotations;
using Hangfire.Dashboard;

public sealed class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context) => true;
}