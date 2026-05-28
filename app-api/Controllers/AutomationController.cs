using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class AutomationController : Controller
{
    private readonly IAutomationRepository automationRepository;
    private readonly IMapper mapper;

    public AutomationController(IAutomationRepository automationRepository, IMapper mapper)
    {
        this.automationRepository = automationRepository;
        this.mapper = mapper;
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

        return Ok(mapper.Map<List<AutomationDTO>>(workspaceAutomations));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAutomation([FromBody] CreateAutomationDTO createAutomationDTO)
    {
        var newAutomation = mapper.Map<Automation>(createAutomationDTO);

        newAutomation = await automationRepository.CreateAutomationAsync(newAutomation);

        if (newAutomation == null)
        {
            return NotFound("Workspace not found!");
        }

        var returnAuto = mapper.Map<AutomationDTO>(newAutomation);

        return CreatedAtAction(nameof(GetAutomationsByWorkspaceId), new { workspaceId = returnAuto.WorkspaceId }, returnAuto);
    }

    [HttpPut]
    [Route("{automationId:guid}")]
    public async Task<IActionResult> UpdateAutomation([FromRoute] Guid automationId, [FromBody] UpdateAutomationDTO updateAutomationDTO)
    {
        var automationToUpdate = mapper.Map<Automation>(updateAutomationDTO);

        automationToUpdate = await automationRepository.UpdateAutomationAsync(automationId, automationToUpdate);

        if (automationToUpdate == null)
        {
            return NotFound("Automation doesn't exist!");
        }

        return Ok(mapper.Map<AutomationDTO>(automationToUpdate));
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
