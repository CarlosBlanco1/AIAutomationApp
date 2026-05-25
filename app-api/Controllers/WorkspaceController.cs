using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly MydbContext _dbContext;
    public WorkspaceController(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<IActionResult> GetWorkspacesByUserId(Guid userId)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == userId);

        if (!userExists)
        {
            return NotFound("User doesn't exist");
        }

        var userWorkspacesDtos = await _dbContext.Workspaces
        .Where(w => w.OwnerId == userId)
        .Select(w => new WorkspaceDTO()
        {
            WorkspaceId = w.WorkspaceId,
            WorkspaceName = w.WorkspaceName,
            OwnerId = w.OwnerId
        })
        .ToListAsync();

        return Ok(userWorkspacesDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == createWorkspaceDTO.OwnerId);

        if (!userExists)
        {
            return NotFound("User doesn't exist");
        }

        var newWorkspace = new Workspace()
        {
            WorkspaceId = Guid.NewGuid(),
            WorkspaceName = createWorkspaceDTO.WorkspaceName,
            OwnerId = createWorkspaceDTO.OwnerId
        };

        await _dbContext.Workspaces.AddAsync(newWorkspace);
        await _dbContext.SaveChangesAsync();

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
        var workspaceToUpdate = await _dbContext.Workspaces.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId);

        if (workspaceToUpdate == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        workspaceToUpdate.WorkspaceName = updateWorkspaceDTO.WorkspaceName;

        await _dbContext.SaveChangesAsync();

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
        var workspaceToDelete = await _dbContext.Workspaces.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId);

        if (workspaceToDelete == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        _dbContext.Workspaces.Remove(workspaceToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}