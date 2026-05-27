using app_api.Models;

public interface IDocumentRepository
{
    Task<List<Document>?> GetDocumentsByWorkspaceIdAsync(Guid workspaceId);
    Task<Document?> CreateDocumentAsync(Document newDocument);
    Task<Document?> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument);
    Task<Document?> DeleteDocumentAsync(Guid DocumentId);
}