using System.Security.Claims;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationLogController : Controller
{
    private readonly IAutomationLogRepository automationLogRepository;
    private readonly IMapper mapper;
    private readonly IAutomationRepository automationRepository;

    public AutomationLogController(IAutomationLogRepository automationLogRepository, IMapper mapper, IAutomationRepository automationRepository)
    {
        this.automationLogRepository = automationLogRepository;
        this.mapper = mapper;
        this.automationRepository = automationRepository;
    }

    [HttpGet]
    [Route("{automationId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAutomationLogsByAutomationId([FromRoute] Guid automationId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var automation = await automationRepository.GetAutomationByIdAsync(automationId);

        if(automation == null)
        {
            return NotFound("Automation doesn't exist!");
        }
        else if(automation.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var automationLogs = await automationLogRepository.GetAutomationLogsByAutomationIdAsync(automationId);

        return Ok(mapper.Map<List<AutomationLogDTO>>(automationLogs));
    }

    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateAutomationLog([FromBody] CreateAutomationLogDTO createAutomationLogDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var automation = await automationRepository.GetAutomationByIdAsync(createAutomationLogDTO.AutomationId);

        if(automation == null)
        {
            return NotFound("Automation doesn't exist!");
        }
        else if(automation.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var newAutomationLog = mapper.Map<AutomationLog>(createAutomationLogDTO);

        newAutomationLog = await automationLogRepository.CreateAutomationLogAsync(newAutomationLog);

        var returnAutomationLog = mapper.Map<AutomationLogDTO>(newAutomationLog);

        return CreatedAtAction(nameof(GetAutomationLogsByAutomationId), new { automationId = returnAutomationLog.AutomationId }, returnAutomationLog);
    }

    [HttpDelete]
    [Route("{automationLogId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAutomationLog([FromRoute] Guid automationLogId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var automationLog = await automationLogRepository.GetAutomationLogByIdAsync(automationLogId);

        if(automationLog == null)
        {
            return NotFound("Automation Log doesn't exist!");
        }
        else if(automationLog.Automation.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        await automationLogRepository.DeleteAutomationLogAsync(automationLogId);

        return Ok();
    }
}