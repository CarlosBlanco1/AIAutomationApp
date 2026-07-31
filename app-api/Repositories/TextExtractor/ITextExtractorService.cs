using Amazon.Util.Internal;
using Pgvector;

public interface ITextExtractorService
{
    Task<List<ChunkResponse>> GetTextEmbeddedChunksAsync(IFormFile file, string fileName);
    Task<Vector> GetEmbeddingForPrompt(string prompt);
}