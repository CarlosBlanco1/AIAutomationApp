using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLDocumentRepository : IDocumentRepository
{
    private readonly MydbContext _dbContext;

    public SQLDocumentRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Document> CreateDocumentAsync(Document newDocument, CancellationToken cancellationToken)
    {
        await _dbContext.Documents.AddAsync(newDocument, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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
            Summary = d.Summary,
            ProcessingStatus = d.ProcessingStatus,
            ProcessingError = d.ProcessingError,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                OwnerId = d.Workspace.OwnerId
            },
            Chunks = new List<Chunk>()
        })
        .FirstOrDefaultAsync();
    }

    public async Task<List<Document>> GetDocumentsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
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
            Summary = d.Summary,
            ProcessingStatus = d.ProcessingStatus,
            ProcessingError = d.ProcessingError,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                OwnerId = d.Workspace.OwnerId
            },
            Chunks = new List<Chunk>()
        })
        .Where(d => d.Workspace.OwnerId == userId)
        .ToListAsync(cancellationToken);
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
            Summary = d.Summary,
            ProcessingStatus = d.ProcessingStatus,
            ProcessingError = d.ProcessingError,
            CreatedAt = d.CreatedAt,
            Workspace = new Workspace
            {
                WorkspaceName = d.Workspace.WorkspaceName,
                WorkspaceId = d.Workspace.WorkspaceId,
                OwnerId = d.Workspace.OwnerId
            },
            Chunks = new List<Chunk>()
        })
        .Where(d => d.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task MarkProcessingFailedAsync(Guid documentId, string error, CancellationToken cancellationToken)
    {
        var documentToMark = await _dbContext.Documents.FirstAsync(d => d.DocumentId == documentId);

        documentToMark.ProcessingStatus = ProcessingStatus.Failed;
        documentToMark.ProcessingError = error;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TryMarkProcessingAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var documentToMark = await _dbContext.Documents.FirstAsync(d => d.DocumentId == documentId);

        if (documentToMark.ProcessingStatus == ProcessingStatus.Pending)
        {
            documentToMark.ProcessingStatus = ProcessingStatus.Processing;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task CompleteProcessingAsync(Guid documentId, string summary, CancellationToken cancellationToken)
    {
        var documentToMark = await _dbContext.Documents.FirstAsync(d => d.DocumentId == documentId);

        documentToMark.ProcessingStatus = ProcessingStatus.Completed;
        documentToMark.Summary = summary;
        documentToMark.ProcessingError = null;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Document> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument)
    {
        var documentToUpdate = await _dbContext.Documents.FirstAsync(d => d.DocumentId == DocumentId);

        documentToUpdate.Description = updatedDocument.Description;
        documentToUpdate.BlobKey = updatedDocument.BlobKey;

        await _dbContext.SaveChangesAsync();

        return documentToUpdate;
    }

    public async Task MarkPendingAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var documentToMark = await _dbContext.Documents.FirstAsync(d => d.DocumentId == documentId);

        documentToMark.ProcessingStatus = ProcessingStatus.Pending;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}