using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLAutomationRepository : IAutomationRepository
{
    private readonly MydbContext _dbContext;

    public SQLAutomationRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Automation?> CreateAutomationAsync(Automation newAutomation)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == newAutomation.WorkspaceId);

        if (!workspaceExists)
        {
            return null;
        }

        await _dbContext.Automations.AddAsync(newAutomation);
        await _dbContext.SaveChangesAsync();

        return newAutomation;
    }

    public async Task<Automation?> DeleteAutomationAsync(Guid AutomationId)
    {
        var automationToDelete = await _dbContext.Automations.FirstOrDefaultAsync(d => d.AutomationId == AutomationId);

        if (automationToDelete == null)
        {
            return null;
        }

        _dbContext.Automations.Remove(automationToDelete);
        await _dbContext.SaveChangesAsync();

        return automationToDelete;
    }

    public async Task<List<Automation>?> GetAutomationsByWorkspaceIdAsync(Guid workspaceId)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return null;
        }

        return await _dbContext.Automations.Where(a => a.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task<Automation?> UpdateAutomationAsync(Guid AutomationId, Automation updatedAutomation)
    {
        var automationToUpdate = await _dbContext.Automations.FirstOrDefaultAsync(a => a.AutomationId == AutomationId);

        if(automationToUpdate == null)
        {
            return null;
        }

        automationToUpdate.AutomationName = updatedAutomation.AutomationName;
        automationToUpdate.TriggerType = updatedAutomation.TriggerType;
        automationToUpdate.ActionType = updatedAutomation.ActionType;
        automationToUpdate.WebhookUrl = updatedAutomation.WebhookUrl;
        automationToUpdate.IsActive = updatedAutomation.IsActive;

        await _dbContext.SaveChangesAsync();

        return automationToUpdate;
    }
}