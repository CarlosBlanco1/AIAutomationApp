using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

public class OllamaChatService : IChatService
{
    private readonly IHttpClientFactory clientFactory;

    public OllamaChatService(IHttpClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    public async Task<string> GenerateSummaryAsync(string fileText)
    {
        var client = clientFactory.CreateClient("ExtendedTimeoutClient");

        JsonSerializerOptions options = JsonSerializerOptions.Default;

        JsonNode schema = options.GetJsonSchemaAsNode(typeof(OllamaSummaryResponse));

        var request = new OllamaSummaryRequest
        {
            Model = "llama3.2:1b",
            Prompt = $"""
    Summarize the following text.

    Return only JSON matching this schema:
    {schema}

    Text:
    {fileText}
    """,
            Format = schema,
            Stream = false
        };

        // var response = await client.PostAsJsonAsync("http://workspaceai-ollama-svc:11434/api/generate", request);
        var response = await client.PostAsJsonAsync("http://ollama:11434/api/generate", request);

        var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

        var summary = JsonSerializer.Deserialize<OllamaSummaryResponse>(
            ollamaResult!.Response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return summary.Summary;
    }

    public async IAsyncEnumerable<OllamaChatResponse> ChatAsync(string message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new OllamaChatRequest
        {
            Model = "llama3.2:1b",
            Prompt = message,
            Stream = true
        };

        var client = clientFactory.CreateClient();

        using var httpRequest = new HttpRequestMessage(
        HttpMethod.Post,
        "http://ollama:11434/api/generate")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await client.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(
            cancellationToken);

        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(
                line,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (chunk is null)
                continue;

            yield return chunk;

            if (chunk.Done)
                yield break;
        }
    }
}