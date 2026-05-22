using app_api.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class AutomationController : Controller
{
    private readonly MydbContext _dbContext;

    public AutomationController(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    public IActionResult GetAutomationsByWorkspaceId(Guid workspaceId)
    {
        var workspaceExists = _dbContext.Workspaces.Any(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }

        var workspaceAutos = _dbContext.Automations
        .Where(a => a.WorkspaceId == workspaceId)
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

        return Ok(workspaceAutos);
    }

    [HttpPost]
    public IActionResult CreateAutomation([FromBody] CreateAutomationDTO createAutomationDTO)
    {
        var workspaceExists = _dbContext.Workspaces.Any(w => w.WorkspaceId == createAutomationDTO.WorkspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }

        var newAuto = new Automation()
        {
            AutomationId = Guid.NewGuid(),
            WorkspaceId = createAutomationDTO.WorkspaceId,
            AutomationName = createAutomationDTO.AutomationName,
            TriggerType = createAutomationDTO.TriggerType,
            ActionType = createAutomationDTO.ActionType,
            WebhookUrl = createAutomationDTO.WebhookUrl,
            IsActive = createAutomationDTO.IsActive
        };

        _dbContext.Automations.Add(newAuto);
        _dbContext.SaveChanges();

        var returnAuto = new AutomationDTO()
        {
            AutomationId = newAuto.AutomationId,
            WorkspaceId = newAuto.WorkspaceId,
            AutomationName = newAuto.AutomationName,
            TriggerType = newAuto.TriggerType,
            ActionType = newAuto.ActionType,
            WebhookUrl = newAuto.WebhookUrl,
            IsActive = newAuto.IsActive
        };

        return CreatedAtAction(nameof(GetAutomationsByWorkspaceId), new {workspaceId = returnAuto.WorkspaceId}, returnAuto);
    }

    [HttpPut]
    [Route("{automationId:guid}")]
    public IActionResult UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationDTO updateAutomationDTO)
    {
        var automationToUpdate = _dbContext.Automations.FirstOrDefault(a => a.AutomationId == automationId);

        if(automationToUpdate == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        automationToUpdate.AutomationName = updateAutomationDTO.AutomationName;
        automationToUpdate.TriggerType = updateAutomationDTO.TriggerType;
        automationToUpdate.ActionType = updateAutomationDTO.ActionType;
        automationToUpdate.WebhookUrl = updateAutomationDTO.WebhookUrl;
        automationToUpdate.IsActive = updateAutomationDTO.IsActive;

        _dbContext.SaveChanges();

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
    public IActionResult DeleteAutomation([FromRoute] Guid automationId)
    {
        var automationToDelete = _dbContext.Automations.FirstOrDefault(d => d.AutomationId == automationId);

        if(automationToDelete == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        _dbContext.Automations.Remove(automationToDelete);
        _dbContext.SaveChanges();

        return Ok();
    }
}
