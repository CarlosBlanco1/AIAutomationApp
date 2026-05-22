using app_api.Models;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult GetAutomationLogsByAutomationId([FromRoute] Guid automationId)
    {
        var automationExists = _dbContext.Automations.Any(a => a.AutomationId == automationId);

        if (!automationExists)
        {
            return NotFound("Automation doesn't exist!");
        }

        var automationLogs = _dbContext.AutomationLogs
        .Where(a => a.AutomationId == automationId)
        .Select(a => new AutomationLogDTO()
        {
            AutomationLogId = a.AutomationLogId,
            AutomationId = a.AutomationId,
            LogStatus = a.LogStatus,
            LogMessage = a.LogMessage,
            CreatedAt = a.CreatedAt
        })
        .ToList();

        return Ok(automationLogs);
    }

    [HttpPost]
    public IActionResult CreateAutomationLog([FromBody] CreateAutomationLogDTO createAutomationLogDTO)
    {
        var automationExists = _dbContext.Automations.Any(a => a.AutomationId == createAutomationLogDTO.AutomationId);

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

        _dbContext.AutomationLogs.Add(newAutomationLog);
        _dbContext.SaveChanges();

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
    public IActionResult DeleteAutomationLog([FromRoute] Guid automationLogId)
    {
        var automationLogToDelete = _dbContext.AutomationLogs.FirstOrDefault(d => d.AutomationLogId == automationLogId);

        if(automationLogToDelete == null)
        {
            return NotFound("Automation Log doesn't exist!");
        }

        _dbContext.AutomationLogs.Remove(automationLogToDelete);
        _dbContext.SaveChanges();

        return Ok();
    }
}