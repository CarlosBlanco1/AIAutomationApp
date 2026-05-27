using app_api.Models;

public interface IAutomationRepository
{
    Task<List<Automation>?> GetAutomationsByWorkspaceIdAsync(Guid workspaceId);
    Task<Automation?> CreateAutomationAsync(Automation newAutomation);
    Task<Automation?> UpdateAutomationAsync(Guid AutomationId, Automation updatedAutomation);
    Task<Automation?> DeleteAutomationAsync(Guid AutomationId);
}