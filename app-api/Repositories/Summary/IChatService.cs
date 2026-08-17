public interface IChatService
{
    Task<string> GenerateSummaryAsync(List<ChunkResponse> fileChunks, CancellationToken cancellationToken = default);
    IAsyncEnumerable<OllamaChatResponse> ChatAsync(string message, Guid documentId, CancellationToken cancellationToken = default);
}