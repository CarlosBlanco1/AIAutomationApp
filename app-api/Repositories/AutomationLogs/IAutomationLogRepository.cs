using app_api.Models;

public interface IAutomationLogRepository
{
    Task<List<AutomationLog>?> GetAutomationLogsByAutomationIdAsync(Guid automationId);
    Task<AutomationLog?> CreateAutomationLogAsync(AutomationLog newAutomationLog);
    Task<AutomationLog?> DeleteAutomationLogAsync(Guid AutomationLogId);
}