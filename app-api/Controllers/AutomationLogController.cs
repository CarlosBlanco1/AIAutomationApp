using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationLogController : Controller
{
    private readonly IAutomationLogRepository automationLogRepository;
    private readonly IMapper mapper;

    public AutomationLogController(IAutomationLogRepository automationLogRepository, IMapper mapper)
    {
        this.automationLogRepository = automationLogRepository;
        this.mapper = mapper;
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

        return Ok(mapper.Map<List<AutomationLogDTO>>(automationLogs));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomationLog([FromBody] CreateAutomationLogDTO createAutomationLogDTO)
    {
        var newAutomationLog = mapper.Map<AutomationLog>(createAutomationLogDTO);

        newAutomationLog = await automationLogRepository.CreateAutomationLogAsync(newAutomationLog);

        if (newAutomationLog == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        var returnAutomationLog = mapper.Map<AutomationLogDTO>(newAutomationLog);

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