using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLAutomationLogRepository : IAutomationLogRepository
{
    private readonly MydbContext _dbContext;

    public SQLAutomationLogRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AutomationLog> CreateAutomationLogAsync(AutomationLog newAutomationLog)
    {
        await _dbContext.AutomationLogs.AddAsync(newAutomationLog);
        await _dbContext.SaveChangesAsync();

        return newAutomationLog;
    }

    public async Task DeleteAutomationLogAsync(Guid AutomationLogId)
    {
        var automationLogToDelete = await _dbContext.AutomationLogs.FirstAsync(d => d.AutomationLogId == AutomationLogId);

        _dbContext.AutomationLogs.Remove(automationLogToDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<AutomationLog?> GetAutomationLogByIdAsync(Guid automationLogId)
    {
        return await _dbContext.AutomationLogs
        .Include(al => al.Automation)
        .ThenInclude(a => a.Workspace)
        .FirstOrDefaultAsync(a => a.AutomationLogId == automationLogId);
    }

    public async Task<List<AutomationLog>> GetAutomationLogsByAutomationIdAsync(Guid automationId)
    {
        var automationLogs = await _dbContext.AutomationLogs.Where(a => a.AutomationId == automationId).ToListAsync();

        return automationLogs;
    }
}