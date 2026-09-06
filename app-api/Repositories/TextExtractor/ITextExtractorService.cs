using Amazon.Util.Internal;
using Pgvector;

public interface ITextExtractorService
{
    Task<List<ChunkResponse>> GetTextEmbeddedChunksAsync(System.IO.Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<Vector> GetEmbeddingForPrompt(string prompt);
}