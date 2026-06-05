using System.Security.Claims;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationController : Controller
{
    private readonly IAutomationRepository automationRepository;
    private readonly IMapper mapper;
    private readonly IWorkspaceRepository workspaceRepository;

    public AutomationController(IAutomationRepository automationRepository, IMapper mapper, IWorkspaceRepository workspaceRepository)
    {
        this.automationRepository = automationRepository;
        this.mapper = mapper;
        this.workspaceRepository = workspaceRepository;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAutomationsByWorkspaceId(Guid workspaceId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspace = await workspaceRepository.GetWorkspaceByIdAsync(workspaceId);

        if (workspace == null)
        {
            return NotFound("Workspace not found!");
        }
        else if(workspace.OwnerId != idInToken)
        {
            return Forbid();
        }
        
        var workspaceAutomations = await automationRepository.GetAutomationsByWorkspaceIdAsync(workspaceId);

        return Ok(mapper.Map<List<AutomationDTO>>(workspaceAutomations));
    }

    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateAutomation([FromBody] CreateAutomationDTO createAutomationDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspace = await workspaceRepository.GetWorkspaceByIdAsync(createAutomationDTO.WorkspaceId);

        if (workspace == null)
        {
            return NotFound("Workspace not found!");
        }
        else if(workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var newAutomation = mapper.Map<Automation>(createAutomationDTO);

        newAutomation = await automationRepository.CreateAutomationAsync(newAutomation);

        var returnAuto = mapper.Map<AutomationDTO>(newAutomation);

        return CreatedAtAction(nameof(GetAutomationsByWorkspaceId), new { workspaceId = returnAuto.WorkspaceId }, returnAuto);
    }

    [HttpPut]
    [ValidateModel]
    [Route("{automationId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationDTO updateAutomationDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var automation = await automationRepository.GetAutomationByIdAsync(automationId);

        if (automation == null)
        {
            return NotFound("Automation not found!");
        }
        else if(automation.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var automationToUpdate = mapper.Map<Automation>(updateAutomationDTO);

        automationToUpdate = await automationRepository.UpdateAutomationAsync(automationId, automationToUpdate);

        return Ok(mapper.Map<AutomationDTO>(automationToUpdate));
    }
    [HttpDelete]
    [Route("{automationId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAutomation([FromRoute] Guid automationId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var automation = await automationRepository.GetAutomationByIdAsync(automationId);

        if (automation == null)
        {
            return NotFound("Automation not found!");
        }
        else if(automation.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        await automationRepository.DeleteAutomationAsync(automationId);

        return Ok();
    }
}
