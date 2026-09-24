using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using app_api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ApiControllerBase
{
    private readonly IWorkspaceRepository workspaceRepository;
    private readonly IIdempotencyRecordRepository idempotencyRecordRepository;
    private readonly IMapper mapper;
    private readonly ILogger logger;

    public WorkspaceController(IWorkspaceRepository workspaceRepository, IIdempotencyRecordRepository idempotencyRecordRepository, IMapper mapper, ILogger logger)
    {
        this.workspaceRepository = workspaceRepository;
        this.idempotencyRecordRepository = idempotencyRecordRepository;
        this.mapper = mapper;
        this.logger = logger;
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
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDTO createWorkspaceDTO, CancellationToken cancellationToken)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var previousRequest = await idempotencyRecordRepository.FetchIdempotencyRecordByKeyAsync(
        idInToken,
        IdempotencyRecordOperation.CreateWorkspace,
        createWorkspaceDTO.RequestKey);

        var requestBodyHash = await ComputeCreateWorkspaceRequestHashAsync(createWorkspaceDTO, cancellationToken);

        if (previousRequest is not null)
        {
            return ComputeObjectResultForDuplicateRequest(previousRequest, requestBodyHash);
        }

        (IdempotencyRecord entry, bool succeeded) newIdempotencyRecord = await idempotencyRecordRepository.CreateIdempotencyRecordAsync(idInToken, IdempotencyRecordOperation.CreateWorkspace, createWorkspaceDTO.RequestKey, requestBodyHash, cancellationToken);

        if (!newIdempotencyRecord.succeeded)
        {
            var duplicateRequest = await idempotencyRecordRepository.FetchIdempotencyRecordByKeyAsync(idInToken, IdempotencyRecordOperation.CreateWorkspace, createWorkspaceDTO.RequestKey, cancellationToken);
            return ComputeObjectResultForDuplicateRequest(duplicateRequest!, requestBodyHash);
        }

        var newWorkspace = mapper.Map<Workspace>(createWorkspaceDTO);

        try
        {
            newWorkspace = await workspaceRepository.CreateWorkspaceAsync(idInToken, newWorkspace);

            if (newWorkspace == null)
            {
                try
                {
                    await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(newIdempotencyRecord.entry.Id);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while deleting failed idempotency record for workspace creation.");
                }

                return NotFound("User doesn't exist");
            }
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (newWorkspace is not null && newWorkspace.WorkspaceId != Guid.Empty)
                {
                    await workspaceRepository.DeleteWorkspaceAsync(newWorkspace.WorkspaceId);
                }
                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(newIdempotencyRecord.entry.Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while cleaning up after request cancellation");
            }
            throw;
        }
        catch (Exception ex)
        {
            try
            {
                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(newIdempotencyRecord.entry.Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while deleting failed idempotency record for workspace creation.");
            }
            logger.LogError(ex, "Error while creating a new workspace");

            return Problem("Could not create workspace");
        }

        var returnWorkspaceDto = mapper.Map<WorkspaceDTO>(newWorkspace);

        try
        {
            await idempotencyRecordRepository.MarkIdempotencyRecordAsCompleteAsync(newIdempotencyRecord.entry.Id, StatusCodes.Status201Created, JsonSerializer.Serialize(returnWorkspaceDto), cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while marking idempotency record as complete in workspace controller");

            try
            {
                await workspaceRepository.DeleteWorkspaceAsync(newWorkspace.WorkspaceId);
                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(newIdempotencyRecord.entry.Id);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(cleanupException, "Error while cleaning up failed workspace creation.");
            }

            return Problem("Could not create workspace.");
        }

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

        if (workspaceToUpdate.OwnerId != idInToken)
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

        if (workspaceToUpdate.OwnerId != idInToken)
        {
            return Forbid();
        }

        await workspaceRepository.DeleteWorkspaceAsync(workspaceId);

        return Ok();
    }

    private static async Task<string> ComputeCreateWorkspaceRequestHashAsync(CreateWorkspaceDTO request, CancellationToken cancellationToken)
    {
        var jsonSerializedRequest = JsonSerializer.Serialize(new
        {
            request.WorkspaceName
        });

        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(jsonSerializedRequest)));
    }
}