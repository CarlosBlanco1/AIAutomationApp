using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLAutomationRepository : IAutomationRepository
{
    private readonly MydbContext _dbContext;

    public SQLAutomationRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Automation> CreateAutomationAsync(Automation newAutomation)
    {
        await _dbContext.Automations.AddAsync(newAutomation);
        await _dbContext.SaveChangesAsync();

        return newAutomation;
    }

    public async Task DeleteAutomationAsync(Guid AutomationId)
    {
        var automationToDelete = await _dbContext.Automations.FirstAsync(d => d.AutomationId == AutomationId);

        _dbContext.Automations.Remove(automationToDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Automation?> GetAutomationByIdAsync(Guid automationId)
    {
        return await _dbContext.Automations
        .Where(a => a.AutomationId == automationId)
        .Select(a => new Automation
        {
            AutomationId = a.AutomationId,
            WorkspaceId = a.Workspace.WorkspaceId,
            AutomationName = a.AutomationName,
            TriggerType = a.TriggerType,
            ActionType = a.ActionType,
            WebhookUrl = a.WebhookUrl,
            IsActive = a.IsActive
        })
        .FirstOrDefaultAsync();
    }

    public async Task<List<Automation>> GetAutomationsByWorkspaceIdAsync(Guid workspaceId)
    {
        return await _dbContext.Automations.Where(a => a.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task<Automation> UpdateAutomationAsync(Guid AutomationId, Automation updatedAutomation)
    {
        var automationToUpdate = await _dbContext.Automations.FirstAsync(a => a.AutomationId == AutomationId);

        automationToUpdate.AutomationName = updatedAutomation.AutomationName;
        automationToUpdate.TriggerType = updatedAutomation.TriggerType;
        automationToUpdate.ActionType = updatedAutomation.ActionType;
        automationToUpdate.WebhookUrl = updatedAutomation.WebhookUrl;
        automationToUpdate.IsActive = updatedAutomation.IsActive;

        await _dbContext.SaveChangesAsync();

        return automationToUpdate;
    }
}