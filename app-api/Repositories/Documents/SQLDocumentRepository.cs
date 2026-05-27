using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLDocumentRepository : IDocumentRepository
{
    private readonly MydbContext _dbContext;

    public SQLDocumentRepository(MydbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Document?> CreateDocumentAsync(Document newDocument)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == newDocument.WorkspaceId);

        if (!workspaceExists)
        {
            return null;
        }

        await _dbContext.Documents.AddAsync(newDocument);
        await _dbContext.SaveChangesAsync();

        return newDocument;
    }

    public async Task<Document?> DeleteDocumentAsync(Guid DocumentId)
    {
        var documentToDelete = await _dbContext.Documents.FirstOrDefaultAsync(d => d.DocumentId == DocumentId);

        if(documentToDelete == null)
        {
            return null;
        }

        _dbContext.Documents.Remove(documentToDelete);
        await _dbContext.SaveChangesAsync();

        return documentToDelete;
    }

    public async Task<List<Document>?> GetDocumentsByWorkspaceIdAsync(Guid workspaceId)
    {
        var workspaceExists = await _dbContext.Workspaces.AnyAsync(w => w.WorkspaceId == workspaceId);

        if (!workspaceExists)
        {
            return null;
        }

        return await _dbContext.Documents.Where(d => d.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task<Document?> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument)
    {
        var documentToUpdate = await _dbContext.Documents.FirstOrDefaultAsync(d => d.DocumentId == DocumentId);

        if(documentToUpdate == null)
        {
            return null;
        }

        documentToUpdate.FileName = updatedDocument.FileName;
        
        await _dbContext.SaveChangesAsync();

        return documentToUpdate;
    }
}