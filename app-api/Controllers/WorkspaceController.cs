using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceRepository workspaceRepository;
    private readonly IMapper mapper;

    public WorkspaceController(IWorkspaceRepository workspaceRepository, IMapper mapper)
    {
        this.workspaceRepository = workspaceRepository;
        this.mapper = mapper;
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

        return Ok(mapper.Map<List<WorkspaceDTO>>(userWorkspaces));
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO)
    {
        var newWorkspace = mapper.Map<Workspace>(createWorkspaceDTO);

        newWorkspace = await workspaceRepository.CreateWorkspaceAsync(newWorkspace);

        if (newWorkspace == null)
        {
            return NotFound("User doesn't exist");
        }

        var returnWorkspaceDto = mapper.Map<WorkspaceDTO>(newWorkspace);

        return CreatedAtAction(nameof(GetWorkspacesByUserId), new { userId = returnWorkspaceDto.OwnerId }, returnWorkspaceDto);
    }

    [HttpPut]
    [ValidateModel]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> UpdateWorkspace(Guid workspaceId, [FromBody] UpdateWorkspaceDTO updateWorkspaceDTO)
    {
        var updatedWorkspace = mapper.Map<Workspace>(updateWorkspaceDTO);

        var workspaceToUpdate = await workspaceRepository.UpdateWorkspaceAsync(workspaceId, updatedWorkspace);

        if (workspaceToUpdate == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        return Ok(mapper.Map<WorkspaceDTO>(workspaceToUpdate));
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