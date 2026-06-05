using app_api.Models;

public interface IAutomationLogRepository
{
    Task<AutomationLog?> GetAutomationLogByIdAsync(Guid automationLogId);
    Task<List<AutomationLog>> GetAutomationLogsByAutomationIdAsync(Guid automationId);
    Task<AutomationLog> CreateAutomationLogAsync(AutomationLog newAutomationLog);
    Task DeleteAutomationLogAsync(Guid AutomationLogId);
}