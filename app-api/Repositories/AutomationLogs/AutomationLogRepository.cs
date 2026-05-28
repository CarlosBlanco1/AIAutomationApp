using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class AutomationLogRepository : IAutomationLogRepository
{
    private readonly MydbContext _dbContext;

    public AutomationLogRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AutomationLog?> CreateAutomationLogAsync(AutomationLog newAutomationLog)
    {
        var automationExists = await _dbContext.Automations.AnyAsync(a => a.AutomationId == newAutomationLog.AutomationId);

        if (!automationExists)
        {
            return null;
        }

        await _dbContext.AutomationLogs.AddAsync(newAutomationLog);
        await _dbContext.SaveChangesAsync();

        return newAutomationLog;
    }

    public async Task<AutomationLog?> DeleteAutomationLogAsync(Guid AutomationLogId)
    {
        var automationLogToDelete = await _dbContext.AutomationLogs.FirstOrDefaultAsync(d => d.AutomationLogId == AutomationLogId);

        if(automationLogToDelete == null)
        {
            return null;
        }

        _dbContext.AutomationLogs.Remove(automationLogToDelete);
        await _dbContext.SaveChangesAsync();

        return automationLogToDelete;
    }

    public async Task<List<AutomationLog>?> GetAutomationLogsByAutomationIdAsync(Guid automationId)
    {
        var automationExists = await _dbContext.Automations.AnyAsync(a => a.AutomationId == automationId);

        if (!automationExists)
        {
            return null;
        }

        var automationLogs = await _dbContext.AutomationLogs.Where(a => a.AutomationId == automationId).ToListAsync();

        return automationLogs;
    }
}