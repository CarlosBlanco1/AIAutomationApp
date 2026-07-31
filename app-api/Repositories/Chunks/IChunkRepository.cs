using Pgvector;

public interface IChunkRepository
{
    Task<List<Chunk>> CreateChunksAsync(List<Chunk> chunks);
    Task<List<Chunk>> RetrieveChunksByDocumentIdAsync(Guid documentId);
    Task<List<string>> GetRelevantChunksForEmbeddingForDocument(Vector queryEmbedding, int tokenBudget, Guid documentId);
}