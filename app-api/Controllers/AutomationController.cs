using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationController : Controller
{
    private readonly IAutomationRepository automationRepository;

    public AutomationController(IAutomationRepository automationRepository)
    {
        this.automationRepository = automationRepository;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> GetAutomationsByWorkspaceId(Guid workspaceId)
    {
        var workspaceAutomations = await automationRepository.GetAutomationsByWorkspaceIdAsync(workspaceId);

        if (workspaceAutomations == null)
        {
            return NotFound("Workspace not found!");
        }

        var workspaceAutomationsDtos = workspaceAutomations
        .Select(a => new AutomationDTO()
        {
            AutomationId = a.AutomationId,
            WorkspaceId = a.WorkspaceId,
            AutomationName = a.AutomationName,
            TriggerType = a.TriggerType,
            ActionType = a.ActionType,
            WebhookUrl = a.WebhookUrl,
            IsActive = a.IsActive
        })
        .ToList();

        return Ok(workspaceAutomationsDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomation([FromBody] CreateAutomationDTO createAutomationDTO)
    {
        var newAutomation = new Automation()
        {
            AutomationId = Guid.NewGuid(),
            WorkspaceId = createAutomationDTO.WorkspaceId,
            AutomationName = createAutomationDTO.AutomationName,
            TriggerType = createAutomationDTO.TriggerType,
            ActionType = createAutomationDTO.ActionType,
            WebhookUrl = createAutomationDTO.WebhookUrl,
            IsActive = createAutomationDTO.IsActive
        };

        newAutomation = await automationRepository.CreateAutomationAsync(newAutomation);

        if (newAutomation == null)
        {
            return NotFound("Workspace not found!");
        }

        var returnAuto = new AutomationDTO()
        {
            AutomationId = newAutomation.AutomationId,
            WorkspaceId = newAutomation.WorkspaceId,
            AutomationName = newAutomation.AutomationName,
            TriggerType = newAutomation.TriggerType,
            ActionType = newAutomation.ActionType,
            WebhookUrl = newAutomation.WebhookUrl,
            IsActive = newAutomation.IsActive
        };

        return CreatedAtAction(nameof(GetAutomationsByWorkspaceId), new { workspaceId = returnAuto.WorkspaceId }, returnAuto);
    }

    [HttpPut]
    [Route("{automationId:guid}")]
    public async Task<IActionResult> UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationDTO updateAutomationDTO)
    {
        var automationToUpdate = new Automation()
        {
            AutomationName = updateAutomationDTO.AutomationName,
            TriggerType = updateAutomationDTO.TriggerType,
            ActionType = updateAutomationDTO.ActionType,
            WebhookUrl = updateAutomationDTO.WebhookUrl,
            IsActive = updateAutomationDTO.IsActive
        };

        automationToUpdate = await automationRepository.UpdateAutomationAsync(automationId, automationToUpdate);

        if (automationToUpdate == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        var automationToReturn = new AutomationDTO()
        {
            AutomationId = automationToUpdate.AutomationId,
            WorkspaceId = automationToUpdate.WorkspaceId,
            AutomationName = automationToUpdate.AutomationName,
            TriggerType = automationToUpdate.TriggerType,
            ActionType = automationToUpdate.ActionType,
            WebhookUrl = automationToUpdate.WebhookUrl,
            IsActive = automationToUpdate.IsActive
        };

        return Ok(automationToReturn);
    }
    [HttpDelete]
    [Route("{automationId:guid}")]
    public async Task<IActionResult> DeleteAutomation([FromRoute] Guid automationId)
    {
        var automationToDelete = await automationRepository.DeleteAutomationAsync(automationId);

        if (automationToDelete == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        return Ok();
    }
}
