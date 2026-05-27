using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : Controller
{
    private readonly IDocumentRepository documentRepository;

    public DocumentController(IDocumentRepository documentRepository)
    {
        this.documentRepository = documentRepository;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    public async Task<IActionResult> GetDocumentsByWorkspaceId(Guid workspaceId)
    {
        var workspaceDocs = await documentRepository.GetDocumentsByWorkspaceIdAsync(workspaceId);

        if (workspaceDocs == null)
        {
            return NotFound("Workspace not found!");
        }
        
        var workspaceDocsDtos = workspaceDocs
        .Select(d => new DocumentDTO()
        {
            DocumentId = d.DocumentId,
            WorkspaceId = d.WorkspaceId,
            FileName = d.FileName,
            FilePath = d.FilePath,
            FileText = d.FileText,
            Summary = d.Summary,
            CreatedAt = d.CreatedAt
        })
        .ToList();

        return Ok(workspaceDocsDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDTO createDocumentDTO)
    {
        var newDoc = new Document()
        {
            DocumentId = Guid.NewGuid(),
            WorkspaceId = createDocumentDTO.WorkspaceId,
            FileName = createDocumentDTO.FileName,
            FilePath = createDocumentDTO.FilePath,
            FileText = createDocumentDTO.FileText,
            Summary = createDocumentDTO.Summary,
            CreatedAt = DateTime.Now
        };

        newDoc = await documentRepository.CreateDocumentAsync(newDoc);

        if (newDoc == null)
        {
            return NotFound("Workspace not found!");
        }

        var returnDocDto = new DocumentDTO()
        {
            DocumentId = newDoc.DocumentId,
            WorkspaceId = newDoc.WorkspaceId,
            FileName = newDoc.FileName,
            FilePath = newDoc.FilePath,
            FileText = newDoc.FileText,
            Summary = newDoc.Summary,
            CreatedAt = newDoc.CreatedAt
        };

        return CreatedAtAction(nameof(GetDocumentsByWorkspaceId), new {workspaceId = newDoc.WorkspaceId}, newDoc);
    }

    [HttpPut]
    [Route("{documentId:guid}")]
    public async Task<IActionResult> UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentDTO updateDocumentDTO)
    {
        var documentToUpdate = new Document()
        {
            FileName = updateDocumentDTO.FileName
        };

        documentToUpdate = await documentRepository.UpdateDocumentAsync(documentId, documentToUpdate);

        if(documentToUpdate == null)
        {
            return NotFound("Document doesn't exist!");
        }

        var documentToReturn = new DocumentDTO()
        {
            DocumentId = documentToUpdate.DocumentId,
            WorkspaceId = documentToUpdate.WorkspaceId,
            FileName = documentToUpdate.FileName,
            FilePath = documentToUpdate.FilePath,
            FileText = documentToUpdate.FileText,
            Summary = documentToUpdate.Summary,
            CreatedAt = documentToUpdate.CreatedAt
        };

        return Ok(documentToReturn);
    }

    [HttpDelete]
    [Route("{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
    {
        var documentToDelete = await documentRepository.DeleteDocumentAsync(documentId);

        if(documentToDelete == null)
        {
            return NotFound("Document doesn't exist!");
        }

        return Ok();
    }
}