using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationLogController : Controller
{
    private readonly MydbContext _dbContext;

    public AutomationLogController(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{automationId:guid}")]
    public async Task<IActionResult> GetAutomationLogsByAutomationId([FromRoute] Guid automationId)
    {
        var automationExists = await _dbContext.Automations.AnyAsync(a => a.AutomationId == automationId);

        if (!automationExists)
        {
            return NotFound("Automation doesn't exist!");
        }

        var automationLogs = await _dbContext.AutomationLogs
        .Where(a => a.AutomationId == automationId)
        .Select(a => new AutomationLogDTO()
        {
            AutomationLogId = a.AutomationLogId,
            AutomationId = a.AutomationId,
            LogStatus = a.LogStatus,
            LogMessage = a.LogMessage,
            CreatedAt = a.CreatedAt
        })
        .ToListAsync();

        return Ok(automationLogs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomationLog([FromBody] CreateAutomationLogDTO createAutomationLogDTO)
    {
        var automationExists = await _dbContext.Automations.AnyAsync(a => a.AutomationId == createAutomationLogDTO.AutomationId);

        if (!automationExists)
        {
            return NotFound("Automation doesn't exist!");
        }

        var newAutomationLog = new AutomationLog()
        {
            AutomationLogId = Guid.NewGuid(),
            AutomationId = createAutomationLogDTO.AutomationId,
            LogStatus = createAutomationLogDTO.LogStatus,
            LogMessage = createAutomationLogDTO.LogMessage,
            CreatedAt = DateTime.Now
        };

        await _dbContext.AutomationLogs.AddAsync(newAutomationLog);
        await _dbContext.SaveChangesAsync();

        var returnAutomationLog = new AutomationLogDTO()
        {
            AutomationLogId = newAutomationLog.AutomationLogId,
            AutomationId = newAutomationLog.AutomationId,
            LogStatus = newAutomationLog.LogStatus,
            LogMessage = newAutomationLog.LogMessage,
            CreatedAt = newAutomationLog.CreatedAt
        };

        return CreatedAtAction(nameof(GetAutomationLogsByAutomationId), new { automationId = returnAutomationLog.AutomationId }, returnAutomationLog);
    }

    [HttpDelete]
    [Route("{automationLogId:guid}")]
    public async Task<IActionResult> DeleteAutomationLog([FromRoute] Guid automationLogId)
    {
        var automationLogToDelete = await _dbContext.AutomationLogs.FirstOrDefaultAsync(d => d.AutomationLogId == automationLogId);

        if(automationLogToDelete == null)
        {
            return NotFound("Automation Log doesn't exist!");
        }

        _dbContext.AutomationLogs.Remove(automationLogToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}