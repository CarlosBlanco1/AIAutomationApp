using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLDocumentRepository : IDocumentRepository
{
    private readonly MydbContext _dbContext;

    public SQLDocumentRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Document> CreateDocumentAsync(Document newDocument)
    {
        await _dbContext.Documents.AddAsync(newDocument);
        await _dbContext.SaveChangesAsync();

        return newDocument;
    }

    public async Task DeleteDocumentAsync(Guid DocumentId)
    {
        var documentToDelete = await _dbContext.Documents.FirstAsync(d => d.DocumentId == DocumentId);

        _dbContext.Documents.Remove(documentToDelete);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
    {
        return await _dbContext.Documents
        .Where(d => d.DocumentId == documentId)
        .Select(d => new Document
        {
            DocumentId = d.DocumentId,
            WorkspaceId = d.WorkspaceId,
            FileName = d.FileName,
            BlobKey = d.BlobKey,
            FileSizeBytes = d.FileSizeBytes,
            Description = d.Description,
            FileText = d.FileText,
            Summary = d.Summary,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                OwnerId = d.Workspace.OwnerId
            }
        })
        .FirstOrDefaultAsync();
    }

    public async Task<List<Document>> GetDocumentsByUserIdAsync(Guid userId)
    {
        return await _dbContext.Documents
        .Select(d => new Document
        {
            DocumentId = d.DocumentId,
            WorkspaceId = d.WorkspaceId,
            FileName = d.FileName,
            BlobKey = d.BlobKey,
            FileSizeBytes = d.FileSizeBytes,
            Description = d.Description,
            FileText = d.FileText,
            Summary = d.Summary,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                OwnerId = d.Workspace.OwnerId
            }
        })
        .Where(d => d.Workspace.OwnerId == userId)
        .ToListAsync();
    }

    public async Task<List<Document>> GetDocumentsByWorkspaceIdAsync(Guid workspaceId)
    {
        return await _dbContext.Documents
        .Select(d => new Document
        {
            DocumentId = d.DocumentId,
            WorkspaceId = d.WorkspaceId,
            FileName = d.FileName,
            BlobKey = d.BlobKey,
            FileSizeBytes = d.FileSizeBytes,
            Description = d.Description,
            FileText = d.FileText,
            Summary = d.Summary,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                WorkspaceId = d.Workspace.WorkspaceId,
                OwnerId = d.Workspace.OwnerId
            }
        })
        .Where(d => d.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task<Document> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument)
    {
        var documentToUpdate = await _dbContext.Documents.FirstAsync(d => d.DocumentId == DocumentId);

        documentToUpdate.Description = updatedDocument.Description;

        await _dbContext.SaveChangesAsync();

        return documentToUpdate;
    }
}