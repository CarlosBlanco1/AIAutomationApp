using app_api.Models;

public interface IDocumentRepository
{
    Task<Document?> GetDocumentByIdAsync(Guid documentId);
    Task<List<Document>> GetDocumentsByWorkspaceIdAsync(Guid workspaceId);
    Task<List<Document>> GetDocumentsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Document> CreateDocumentAsync(Document newDocument, CancellationToken cancellationToken);
    Task<Document> UpdateDocumentAsync(Guid DocumentId, Document updatedDocument);
    Task DeleteDocumentAsync(Guid DocumentId);
}