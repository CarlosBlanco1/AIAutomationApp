using app_api.Models;

public interface IDocumentRepository
{
    Task<Document?> GetDocumentByIdAsync(Guid documentId);
    Task<List<Document>> GetDocumentsByWorkspaceIdAsync(Guid workspaceId);
    Task<List<Document>> GetDocumentsByUserIdAsync(Guid userId);
    Task<Document> CreateDocumentAsync(Document newDocument);
    Task<Document> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument);
    Task DeleteDocumentAsync(Guid DocumentId);
}