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
    private readonly IMapper mapper;

    public DocumentController(IDocumentRepository documentRepository, IWorkspaceRepository workspaceRepository, IFileStorageService storageService, IMapper mapper)
    {
        this.documentRepository = documentRepository;
        this.workspaceRepository = workspaceRepository;
        this.storageService = storageService;
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
        else if(workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var workspaceDocs = await documentRepository.GetDocumentsByWorkspaceIdAsync(workspaceId);
        
        return Ok(mapper.Map<List<DocumentDTO>>(workspaceDocs));
    }

    [HttpPost]
    [ValidateModel]
    [Authorize]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDTO createDocumentDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var workspace = await workspaceRepository.GetWorkspaceByIdAsync(createDocumentDTO.WorkspaceId);

        if(workspace == null)
        {
            return NotFound("Workspace not found!");
        }
        else if(workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        var newDoc = mapper.Map<Document>(createDocumentDTO);

        var blobKey = $"users/{idInToken}/workspaces/{createDocumentDTO.WorkspaceId}/documents/{newDoc.DocumentId}.pdf";

        newDoc.BlobKey = blobKey;

        //STORE IT IN R2
        var uploadFileResult = await storageService.UploadAsync(createDocumentDTO.File, blobKey);

        if(!uploadFileResult.Succeeded)
        {
            return BadRequest(uploadFileResult.Error);
        }
        
        //CALL AI TO GET SUMMARY


        newDoc = await documentRepository.CreateDocumentAsync(newDoc);

        var returnDocDto = mapper.Map<DocumentDTO>(newDoc);

        return CreatedAtAction(nameof(GetDocumentsByWorkspaceId), new {workspaceId = newDoc.WorkspaceId}, newDoc);
    }

    [HttpPut]
    [ValidateModel]
    [Route("{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentDTO updateDocumentDTO)
    {
        var idInToken = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var document = await documentRepository.GetDocumentByIdAsync(documentId);

        if(document == null)
        {
            return NotFound("Document doesn't exist!");
        }
        else if(document.Workspace.OwnerId != idInToken)
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

        if(document == null)
        {
            return NotFound("Document doesn't exist!");
        }
        else if(document.Workspace.OwnerId != idInToken)
        {
            return Forbid();
        }

        await documentRepository.DeleteDocumentAsync(documentId);

        return Ok();
    }
}