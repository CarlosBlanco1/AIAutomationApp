using Pgvector;

public interface IChunkRepository
{
    Task<List<Chunk>> CreateChunksAsync(List<Chunk> chunks, CancellationToken cancellationToken);
    Task<List<Chunk>> RetrieveChunksByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
    Task<List<string>> GetRelevantChunksForEmbeddingForDocument(Vector queryEmbedding, int tokenBudget, Guid documentId, CancellationToken cancellationToken);
}