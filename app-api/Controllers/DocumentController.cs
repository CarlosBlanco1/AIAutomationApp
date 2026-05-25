using app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetDocumentsByWorkspaceId(Guid workspaceId)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return NotFound("Workspace not found!");
        }
        
        var workspaceDocs = await _dbContext.Documents
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
        .ToListAsync();

        return Ok(workspaceDocs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDTO createDocumentDTO)
    {
        var workspaceExists = await _dbContext.Documents.AnyAsync(d => d.WorkspaceId == createDocumentDTO.WorkspaceId);

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

        await _dbContext.Documents.AddAsync(newDoc);
        await _dbContext.SaveChangesAsync();

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
        var documentToUpdate = await _dbContext.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);

        if(documentToUpdate == null)
        {
            return NotFound("Document doesn't exist!");
        }

        documentToUpdate.FileName = updateDocumentDTO.FileName;

        await _dbContext.SaveChangesAsync();

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
        var documentToDelete = await _dbContext.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);

        if(documentToDelete == null)
        {
            return NotFound("Document doesn't exist!");
        }

        _dbContext.Documents.Remove(documentToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}