public interface IChatService
{
    Task<string> GenerateSummaryAsync(List<ChunkResponse> fileChunks);
    IAsyncEnumerable<OllamaChatResponse> ChatAsync(string message, Guid documentId, CancellationToken cancellationToken = default);
}