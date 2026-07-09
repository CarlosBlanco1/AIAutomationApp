using System.Security.Claims;
using app_api.Models;
using AutoMapper;
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
    private readonly ITextExtractorService textExtractorService;
    private readonly ISummaryService summaryService;
    private readonly IMapper mapper;

    public DocumentController(IDocumentRepository documentRepository, IWorkspaceRepository workspaceRepository, IFileStorageService storageService, ITextExtractorService textExtractorService, ISummaryService summaryService, IMapper mapper)
    {
        this.documentRepository = documentRepository;
        this.workspaceRepository = workspaceRepository;
        this.storageService = storageService;
        this.textExtractorService = textExtractorService;
        this.summaryService = summaryService;
        this.mapper = mapper;
    }

    [HttpGet]
    [Route("me")]
    [Authorize]
    public async Task<IActionResult> GetMyDocuments()
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var documents = await documentRepository.GetDocumentsByUserIdAsync(idInToken);

        return Ok(mapper.Map<List<DocumentDTO>>(documents));
    }

    [HttpGet]
    [Route("single-doc/{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentById(Guid documentId)
    {
        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if(document != null)
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
    public async Task<IActionResult> CreateDocument([FromForm] CreateDocumentDTO createDocumentDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var userDocs = await documentRepository.GetDocumentsByUserIdAsync(idInToken);
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
        else if(totalDocsSize > (2 * 1024 * 1024)) //2 MB
        {
            return BadRequest("You have more than 2 MB worth of space occupied, get rid of some of your documents");
        }

        var newDoc = mapper.Map<Document>(createDocumentDTO);

        var blobKey = $"users/{idInToken}/workspaces/{createDocumentDTO.WorkspaceId}/documents/{newDoc.DocumentId}.pdf";

        newDoc.BlobKey = blobKey;
        var file = createDocumentDTO.File;

        var fileText = await textExtractorService.GetTextExtractedAsync(file, file.FileName);

        newDoc.FileText = fileText.text;

        //CALL AI TO GET SUMMARY
        var textSummary = await summaryService.GenerateSummary(fileText.text);

        newDoc.Summary = textSummary;

        //STORE IT IN R2
        var uploadFileResult = await storageService.UploadAsync(file, blobKey);

        if (!uploadFileResult.Succeeded)
        {
            return BadRequest(uploadFileResult.Error);
        }

        newDoc = await documentRepository.CreateDocumentAsync(newDoc);

        var returnDocDto = mapper.Map<DocumentDTO>(newDoc);

        return CreatedAtAction(nameof(GetDocumentsByWorkspaceId), new { workspaceId = createDocumentDTO.WorkspaceId }, newDoc);
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

        var response = await storageService.DeleteAsync(document.BlobKey);

        if (!response.Contains("Successful deletion!"))
        {
            return BadRequest(response);
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