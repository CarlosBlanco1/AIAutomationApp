public interface IChatService
{
    Task<string> GenerateSummaryAsync(string fileText);
    IAsyncEnumerable<OllamaChatResponse> ChatAsync(string message, CancellationToken cancellationToken = default);
}