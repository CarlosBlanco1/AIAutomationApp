using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationLogController : Controller
{
    private readonly IAutomationLogRepository automationLogRepository;

    public AutomationLogController(IAutomationLogRepository automationLogRepository)
    {
        this.automationLogRepository = automationLogRepository;
    }

    [HttpGet]
    [Route("{automationId:guid}")]
    public async Task<IActionResult> GetAutomationLogsByAutomationId([FromRoute] Guid automationId)
    {
        var automationLogs = await automationLogRepository.GetAutomationLogsByAutomationIdAsync(automationId);

        if (automationLogs == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        var automationLogsDtos = automationLogs
        .Select(a => new AutomationLogDTO()
        {
            AutomationLogId = a.AutomationLogId,
            AutomationId = a.AutomationId,
            LogStatus = a.LogStatus,
            LogMessage = a.LogMessage,
            CreatedAt = a.CreatedAt
        })
        .ToList();

        return Ok(automationLogsDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomationLog([FromBody] CreateAutomationLogDTO createAutomationLogDTO)
    {
        var newAutomationLog = new AutomationLog()
        {
            AutomationLogId = Guid.NewGuid(),
            AutomationId = createAutomationLogDTO.AutomationId,
            LogStatus = createAutomationLogDTO.LogStatus,
            LogMessage = createAutomationLogDTO.LogMessage,
            CreatedAt = DateTime.Now
        };

        newAutomationLog = await automationLogRepository.CreateAutomationLogAsync(newAutomationLog);

        if (newAutomationLog == null)
        {
            return NotFound("Automation doesn't exist!");
        }

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
        var automationLogToDelete = await automationLogRepository.DeleteAutomationLogAsync(automationLogId);

        if(automationLogToDelete == null)
        {
            return NotFound("Automation Log doesn't exist!");
        }

        return Ok();
    }
}