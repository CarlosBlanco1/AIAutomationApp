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
    public IActionResult GetWorkspacesByUserId(Guid userId)
    {
        var userExists = _dbContext.Users.Any(u => u.UserId == userId);

        if (!userExists)
        {
            return NotFound("User doesn't exist");
        }

        var userWorkspacesDtos = _dbContext.Workspaces
        .Where(w => w.OwnerId == userId)
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
    public IActionResult CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO)
    {
        var userExists = _dbContext.Users.Any(u => u.UserId == createWorkspaceDTO.OwnerId);

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

        _dbContext.Workspaces.Add(newWorkspace);
        _dbContext.SaveChanges();

        var returnWorkspaceDto = new WorkspaceDTO()
        {
            WorkspaceId = newWorkspace.WorkspaceId,
            WorkspaceName = newWorkspace.WorkspaceName,
            OwnerId = newWorkspace.OwnerId
        };

        return CreatedAtAction(nameof(GetWorkspacesByUserId), new {userId = returnWorkspaceDto.OwnerId}, returnWorkspaceDto);  
    }
}