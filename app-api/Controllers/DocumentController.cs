using System.Security.Claims;
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
    private readonly IMapper mapper;
    private readonly ILogger<DocumentController> logger;

    public DocumentController(IDocumentRepository documentRepository, IWorkspaceRepository workspaceRepository, IFileStorageService storageService, IBackgroundJobClient backgroundJobs, IMapper mapper, ILogger<DocumentController> logger)
    {
        this.documentRepository = documentRepository;
        this.workspaceRepository = workspaceRepository;
        this.storageService = storageService;
        this.backgroundJobs = backgroundJobs;
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
        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if (document != null)
        {
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
        try
        {
            var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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

            var newDoc = mapper.Map<Document>(createDocumentDTO);
            newDoc.ProcessingStatus = ProcessingStatus.Pending;

            try
            {
                newDoc = await documentRepository.CreateDocumentAsync(newDoc, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError("An error ocurrred while creating the new document in the DB! " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not upload document to the DB");
            }


            var file = createDocumentDTO.File;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var blobKey = $"users/{idInToken}/workspaces/{createDocumentDTO.WorkspaceId}/documents/{newDoc.DocumentId}{extension}";

            try
            {
                var uploadResult = await storageService.UploadAsync(file, blobKey, cancellationToken);

                if (!uploadResult.Succeeded)
                {
                    newDoc.ProcessingStatus = ProcessingStatus.Failed;
                    await documentRepository.UpdateDocumentAsync(newDoc.DocumentId, newDoc);

                    return StatusCode(StatusCodes.Status500InternalServerError, uploadResult.Error);
                }

                newDoc.BlobKey = blobKey;
                await documentRepository.UpdateDocumentAsync(newDoc.DocumentId, newDoc);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to upload document or save blob key for document {DocumentId}",
                    newDoc.DocumentId);

                newDoc.ProcessingStatus = ProcessingStatus.Failed;
                await documentRepository.UpdateDocumentAsync(newDoc.DocumentId, newDoc);

                try
                {
                    await storageService.DeleteAsync(blobKey);
                }
                catch (Exception cleanupEx)
                {
                    logger.LogWarning(cleanupEx,
                        "Could not clean up blob {BlobKey}", blobKey);
                }

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not upload the document.");
            }

            backgroundJobs.Enqueue<DocumentProcessingJob>(job => job.ProcessAsync(newDoc.DocumentId, CancellationToken.None));

            return Accepted(new
            {
                documentId = newDoc.DocumentId,
                status = newDoc.ProcessingStatus
            });

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Create document operation cancelled for document {createDocumentDTO.FileName}", createDocumentDTO.FileName);
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

        documentToUpdate = await documentRepository.UpdateDocumentAsync(documentId, documentToUpdate);

        return Ok(mapper.Map<DocumentDTO>(documentToUpdate));
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

        if(document.BlobKey != null)
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

        var url = await storageService.CreateDownloadUrlAsync(document.BlobKey);

        return Ok(new
        {
            downloadUrl = url
        });
    }
}