using app_api.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : Controller
{
    private readonly MydbContext _dbContext;

    public DocumentController(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("{workspaceId:guid}")]
    public IActionResult GetDocumentsByWorkspaceId(Guid workspaceId)
    {
        var workspaceExists = _dbContext.Workspaces.Any(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }
        
        var workspaceDocs = _dbContext.Documents
        .Where(d => d.WorkspaceId == workspaceId)
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

        return Ok(workspaceDocs);
    }

    [HttpPost]
    public IActionResult CreateDocument([FromBody] CreateDocumentDTO createDocumentDTO)
    {
        var workspaceExists = _dbContext.Documents.Any(d => d.WorkspaceId == createDocumentDTO.WorkspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }

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

        _dbContext.Documents.Add(newDoc);
        _dbContext.SaveChanges();

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
    public IActionResult UpdateDocument([FromRoute] Guid documentId, [FromBody] UpdateDocumentDTO updateDocumentDTO)
    {
        var documentToUpdate = _dbContext.Documents.FirstOrDefault(d => d.DocumentId == documentId);

        if(documentToUpdate == null)
        {
            return NotFound("Document doesn't exist!");
        }

        documentToUpdate.FileName = updateDocumentDTO.FileName;

        _dbContext.SaveChanges();

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
    public IActionResult DeleteDocument([FromRoute] Guid documentId)
    {
        var documentToDelete = _dbContext.Documents.FirstOrDefault(d => d.DocumentId == documentId);

        if(documentToDelete == null)
        {
            return NotFound("Document doesn't exist!");
        }

        _dbContext.Documents.Remove(documentToDelete);
        _dbContext.SaveChanges();

        return Ok();
    }
}