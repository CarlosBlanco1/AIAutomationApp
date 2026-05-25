using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetAutomationsByWorkspaceId(Guid workspaceId)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }

        var workspaceAutos = await _dbContext.Automations
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
        .ToListAsync();

        return Ok(workspaceAutos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomation([FromBody] CreateAutomationDTO createAutomationDTO)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == createAutomationDTO.WorkspaceId);

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

        await _dbContext.Automations.AddAsync(newAuto);
        await _dbContext.SaveChangesAsync();

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
    public async Task<IActionResult> UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationDTO updateAutomationDTO)
    {
        var automationToUpdate = await _dbContext.Automations.FirstOrDefaultAsync(a => a.AutomationId == automationId);

        if(automationToUpdate == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        automationToUpdate.AutomationName = updateAutomationDTO.AutomationName;
        automationToUpdate.TriggerType = updateAutomationDTO.TriggerType;
        automationToUpdate.ActionType = updateAutomationDTO.ActionType;
        automationToUpdate.WebhookUrl = updateAutomationDTO.WebhookUrl;
        automationToUpdate.IsActive = updateAutomationDTO.IsActive;

        await _dbContext.SaveChangesAsync();

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
        var automationToDelete = await _dbContext.Automations.FirstOrDefaultAsync(d => d.AutomationId == automationId);

        if(automationToDelete == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        _dbContext.Automations.Remove(automationToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
