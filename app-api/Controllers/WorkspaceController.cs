using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceRepository workspaceRepository;

    public WorkspaceController(IWorkspaceRepository workspaceRepository)
    {
        this.workspaceRepository = workspaceRepository;
    }

    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<IActionResult> GetWorkspacesByUserId(Guid userId)
    {
        var userWorkspaces = await workspaceRepository.GetWorkspacesByUserIdAsync(userId);

        if (userWorkspaces == null)
        {
            return NotFound("User doesn't exist");
        }

        var userWorkspacesDtos = userWorkspaces
        .Select(w => new WorkspaceDTO()
        {
            WorkspaceId = w.WorkspaceId,
            WorkspaceName = w.WorkspaceName,
            OwnerId = w.OwnerId
        })
        .ToList();

        return Ok(userWorkspacesDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO)
    {
        var newWorkspace = new Workspace()
        {
            WorkspaceId = Guid.NewGuid(),
            WorkspaceName = createWorkspaceDTO.WorkspaceName,
            OwnerId = createWorkspaceDTO.OwnerId
        };

        newWorkspace = await workspaceRepository.CreateWorkspaceAsync(newWorkspace);

        if (newWorkspace == null)
        {
            return NotFound("User doesn't exist");
        }

        var returnWorkspaceDto = new WorkspaceDTO()
        {
            WorkspaceId = newWorkspace.WorkspaceId,
            WorkspaceName = newWorkspace.WorkspaceName,
            OwnerId = newWorkspace.OwnerId
        };

        return CreatedAtAction(nameof(GetWorkspacesByUserId), new { userId = returnWorkspaceDto.OwnerId }, returnWorkspaceDto);
    }

    [HttpPut]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> UpdateWorkspace(Guid workspaceId, [FromBody] UpdateWorkspaceDTO updateWorkspaceDTO)
    {
        var updatedWorkspace = new Workspace()
        {
            WorkspaceName = updateWorkspaceDTO.WorkspaceName
        };

        var workspaceToUpdate = await workspaceRepository.UpdateWorkspaceAsync(workspaceId, updatedWorkspace);

        if (workspaceToUpdate == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        var workspaceToReturn = new Workspace()
        {
            WorkspaceId = workspaceToUpdate.WorkspaceId,
            WorkspaceName = workspaceToUpdate.WorkspaceName,
            OwnerId = workspaceToUpdate.OwnerId
        };

        return Ok(workspaceToReturn);
    }

    [HttpDelete]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> DeleteWorkspace([FromRoute] Guid workspaceId)
    {
        var workspaceToDelete = await workspaceRepository.DeleteWorkspaceAsync(workspaceId);

        if (workspaceToDelete == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        return Ok();
    }
}