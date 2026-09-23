using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Amazon.S3.Model;
using app_api.Models;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : Controller
{
    private readonly IDocumentRepository documentRepository;
    private readonly IWorkspaceRepository workspaceRepository;
    private readonly IFileStorageService storageService;
    private readonly IBackgroundJobClient backgroundJobs;
    private readonly IIdempotencyRecordRepository idempotencyRecordRepository;
    private readonly IMapper mapper;
    private readonly ILogger<DocumentController> logger;

    public DocumentController(IDocumentRepository documentRepository,
    IWorkspaceRepository workspaceRepository,
    IFileStorageService storageService,
    IBackgroundJobClient backgroundJobs,
    IIdempotencyRecordRepository idempotencyRecordRepository,
    IMapper mapper,
    ILogger<DocumentController> logger)
    {
        this.documentRepository = documentRepository;
        this.workspaceRepository = workspaceRepository;
        this.storageService = storageService;
        this.backgroundJobs = backgroundJobs;
        this.idempotencyRecordRepository = idempotencyRecordRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    [HttpGet]
    [Route("me")]
    [Authorize]
    public async Task<IActionResult> GetMyDocuments()
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var documents = await documentRepository.GetDocumentsByUserIdAsync(idInToken, CancellationToken.None);

        return Ok(mapper.Map<List<DocumentDTO>>(documents));
    }

    [HttpGet]
    [Route("single-doc/{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentById(Guid documentId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if (document != null)
        {
            if (document.Workspace.OwnerId != idInToken)
            {
                return Forbid();
            }

            return Ok(mapper.Map<DocumentDTO>(document));
        }
        else
        {
            return NotFound("Document with specified Id doesn't exist");
        }
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentsByWorkspaceId(Guid workspaceId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspace = await workspaceRepository.GetWorkspaceByIdAsync(workspaceId);

        if (workspace == null)
        {
            return NotFound("Workspace not found!");
        }
        else if (workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var workspaceDocs = await documentRepository.GetDocumentsByWorkspaceIdAsync(workspaceId);

        return Ok(mapper.Map<List<DocumentDTO>>(workspaceDocs));
    }

    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateDocument([FromForm] CreateDocumentDTO createDocumentDTO, CancellationToken cancellationToken)
    {
        Guid? deleteableIdempotencyRecordId = null;
        Guid? deleteableDocumentId = null;
        string? deleteableBlobKey = null;
        var wasJobEnqueued = false;

        try
        {
            var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var previousRequest = await idempotencyRecordRepository.FetchIdempotencyRecordByKeyAsync(
            idInToken,
            IdempotencyRecordOperation.CreateDocument,
            createDocumentDTO.RequestKey);

            var requestBodyHash = await ComputeCreateDocumentRequestHashAsync(createDocumentDTO, cancellationToken);

            if (previousRequest is not null) return ComputeObjectResultForDuplicateRequest(previousRequest, requestBodyHash);

            var userDocs = await documentRepository.GetDocumentsByUserIdAsync(idInToken, cancellationToken);
            var totalDocsSize = userDocs.Sum(d => d.FileSizeBytes);

            var workspace = await workspaceRepository.GetWorkspaceByIdAsync(createDocumentDTO.WorkspaceId);

            if (workspace == null)
            {
                return NotFound("Workspace not found!");
            }
            else if (workspace.OwnerId != idInToken)
            {
                return Forbid();
            }
            else if ((totalDocsSize + createDocumentDTO.File.Length) > (1 * 1024 * 1024))
            {
                return BadRequest("You have more than 1 MB worth of space occupied, get rid of some of your documents.");
            }

            (IdempotencyRecord entry, bool succeded) newIdempotencyRecord = await idempotencyRecordRepository.CreateIdempotencyRecordAsync(idInToken, IdempotencyRecordOperation.CreateDocument, createDocumentDTO.RequestKey, requestBodyHash, cancellationToken);

            if (!newIdempotencyRecord.succeded)
            {
                var duplicateRequest = await idempotencyRecordRepository.FetchIdempotencyRecordByKeyAsync(idInToken, IdempotencyRecordOperation.CreateDocument, createDocumentDTO.RequestKey, cancellationToken);
                return ComputeObjectResultForDuplicateRequest(duplicateRequest!, requestBodyHash);
            }

            deleteableIdempotencyRecordId = newIdempotencyRecord.entry.Id;

            var newDoc = mapper.Map<Document>(createDocumentDTO);
            newDoc.ProcessingStatus = ProcessingStatus.Pending;

            try
            {
                newDoc = await documentRepository.CreateDocumentAsync(newDoc, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurrred while creating the new document in the DB! ");
                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(deleteableIdempotencyRecordId.Value, cancellationToken);
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not upload document to the DB");
            }

            deleteableDocumentId = newDoc.DocumentId;

            var file = createDocumentDTO.File;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var blobKey = $"users/{idInToken}/workspaces/{createDocumentDTO.WorkspaceId}/documents/{newDoc.DocumentId}{extension}";
            deleteableBlobKey = blobKey;

            try
            {
                var uploadResult = await storageService.UploadAsync(file, blobKey, cancellationToken);

                if (!uploadResult.Succeeded)
                {
                    await documentRepository.DeleteDocumentAsync(newDoc.DocumentId);
                    await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(deleteableIdempotencyRecordId.Value, cancellationToken);

                    return StatusCode(StatusCodes.Status500InternalServerError, uploadResult.Error);
                }

                newDoc.BlobKey = blobKey;
                await documentRepository.UpdateDocumentAsync(newDoc.DocumentId, newDoc);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to upload document or save blob key for document {DocumentId}",
                    newDoc.DocumentId);

                await documentRepository.DeleteDocumentAsync(newDoc.DocumentId);

                try
                {
                    await storageService.DeleteAsync(blobKey);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception cleanupEx)
                {
                    logger.LogWarning(cleanupEx, "Could not clean up blob {BlobKey}", blobKey);
                    await CleanupAsync(deleteableIdempotencyRecordId, deleteableDocumentId, deleteableBlobKey);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not delete the blob");

                }

                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(deleteableIdempotencyRecordId.Value, cancellationToken);

                return StatusCode(StatusCodes.Status500InternalServerError, "Could not upload the document.");
            }

            try
            {
                backgroundJobs.Enqueue<DocumentProcessingJob>(job => job.ProcessAsync(newDoc.DocumentId, CancellationToken.None));
                wasJobEnqueued = true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not enqueue processing for document {DocumentId}", newDoc.DocumentId);
                await CleanupAsync(deleteableIdempotencyRecordId, deleteableDocumentId, deleteableBlobKey);
                throw;
            }

            try
            {
                var responseBody = new
                {
                    documentId = newDoc.DocumentId,
                    status = newDoc.ProcessingStatus
                };

                await idempotencyRecordRepository.MarkIdempotencyRecordAsCompleteAsync(newIdempotencyRecord.entry.Id, StatusCodes.Status202Accepted, JsonSerializer.Serialize(responseBody), CancellationToken.None);
                return Accepted(responseBody);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Document {DocumentId} was queued, but its idempotency record could not be completed.", newDoc.DocumentId);
                throw;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Create document operation cancelled for document {createDocumentDTO.FileName}", createDocumentDTO.FileName);
            if (!wasJobEnqueued)
            {
                await CleanupAsync(deleteableIdempotencyRecordId, deleteableDocumentId, deleteableBlobKey);
            }
            throw;
        }
    }

    [HttpPut]
    [ValidateModel]
    [Route("{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentDTO updateDocumentDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if (document == null)
        {
            return NotFound("Document doesn't exist!");
        }
        else if (document.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var documentToUpdate = mapper.Map<Document>(updateDocumentDTO);

        var updatedDocument = await documentRepository.UpdateDocumentAsync(documentId, documentToUpdate);

        return Ok(mapper.Map<DocumentDTO>(updatedDocument));
    }

    [HttpDelete]
    [Route("{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if (document == null)
        {
            return NotFound("Document doesn't exist!");
        }
        else if (document.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        if (document.BlobKey != null)
        {
            var response = await storageService.DeleteAsync(document.BlobKey);

            if (!response.Contains("Successful deletion!"))
            {
                return BadRequest(response);
            }
        }


        await documentRepository.DeleteDocumentAsync(documentId);

        return Ok();
    }

    [HttpGet]
    [Route("download-url/{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentDownloadUrlAsync([FromRoute] Guid documentId)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if (document == null)
        {
            return NotFound("Document doesn't exist!");
        }
        else if (document.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var url = await storageService.CreateDownloadUrlAsync(document.BlobKey!);

        return Ok(new
        {
            downloadUrl = url
        });
    }

    private static async Task<string> ComputeCreateDocumentRequestHashAsync(CreateDocumentDTO request, CancellationToken cancellationToken)
    {
        await using var fileStream = request.File.OpenReadStream();
        var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);

        var fileHash = Convert.ToBase64String(hashBytes);

        var jsonSerializedRequest = JsonSerializer.Serialize(new
        {
            request.WorkspaceId,
            request.FileName,
            request.Description,
            FileHash = fileHash
        });

        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(jsonSerializedRequest)));
    }

    private async Task CleanupAsync(Guid? idempotencyRecordId, Guid? documentId, string? blobKey)
    {
        try
        {
            if (documentId.HasValue)
            {
                await documentRepository.DeleteDocumentAsync(documentId.Value);
            }

            if (blobKey != null)
            {
                await storageService.DeleteAsync(blobKey);
            }

            if (idempotencyRecordId.HasValue)
            {
                await idempotencyRecordRepository.DeleteFailedIdempotencyRecordAsync(idempotencyRecordId.Value);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred on cleanup after cancellation");
        }
    }

    private ObjectResult ComputeObjectResultForDuplicateRequest(IdempotencyRecord previousRequest, string requestBodyHash)
    {
        if (requestBodyHash != previousRequest.RequestBodyHash)
        {
            return Conflict("An idempotency key was reused with a different request body");
        }
        else if (previousRequest.Status == IdempotencyRecordStatus.Processing)
        {
            return Conflict("An identical request is already being processed");
        }
        else if (previousRequest.ResponseStatusCode is not null && previousRequest.ResponseBody is not null)
        {
            return StatusCode(previousRequest.ResponseStatusCode.Value, JsonSerializer.Deserialize<object>(previousRequest.ResponseBody));
        }
        else
        {
            throw new Exception("Previous Request not in valid state!");
        }
    }
}