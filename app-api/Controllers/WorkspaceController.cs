using System.Security.Claims;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<IActionResult> GetWorkspacesByUserId()
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userWorkspaces = await workspaceRepository.GetWorkspacesByUserIdAsync(idInToken);

        if (userWorkspaces == null)
        {
            return NotFound("User doesn't exist");
        }

        return Ok(mapper.Map<List<WorkspaceDTO>>(userWorkspaces));
    }

    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var newWorkspace = mapper.Map<Workspace>(createWorkspaceDTO);

        newWorkspace = await workspaceRepository.CreateWorkspaceAsync(idInToken, newWorkspace);

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
    [Authorize]
    public async Task<IActionResult> UpdateWorkspace(Guid workspaceId, [FromBody] UpdateWorkspaceDTO updateWorkspaceDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspaceToUpdate = await workspaceRepository.GetWorkspaceByIdAsync(workspaceId);

        if (workspaceToUpdate == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        if(workspaceToUpdate.OwnerId != idInToken)
        {
            return Forbid();
        }
        
        var updatedWorkspace = mapper.Map<Workspace>(updateWorkspaceDTO);

        workspaceToUpdate = await workspaceRepository.UpdateWorkspaceAsync(workspaceId, updatedWorkspace);

        return Ok(mapper.Map<WorkspaceDTO>(workspaceToUpdate));
    }

    [HttpDelete]
    [Route("{workspaceId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteWorkspace([FromRoute] Guid workspaceId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspaceToUpdate = await workspaceRepository.GetWorkspaceByIdAsync(workspaceId);

        if (workspaceToUpdate == null)
        {
            return NotFound("Workspace doesn't exist!");
        }

        if(workspaceToUpdate.OwnerId != idInToken)
        {
            return Forbid();
        }

        await workspaceRepository.DeleteWorkspaceAsync(workspaceId);

        return Ok();
    }
}