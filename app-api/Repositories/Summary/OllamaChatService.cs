using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

public class OllamaChatService : IChatService
{
    private readonly IHttpClientFactory clientFactory;
    private readonly IChunkRepository chunkRepository;
    private readonly ITextExtractorService textExtractorService;
    private readonly string ollamaBaseUrl;

    public OllamaChatService(IHttpClientFactory clientFactory, IChunkRepository chunkRepository, ITextExtractorService textExtractorService, IConfiguration configuration)
    {
        this.clientFactory = clientFactory;
        this.chunkRepository = chunkRepository;
        this.textExtractorService = textExtractorService;
        ollamaBaseUrl = configuration["OLLAMA_BASE_URL"]!;
    }

    public async Task<string> GenerateSummaryAsync(List<ChunkResponse> fileChunks)
    {
        var client = clientFactory.CreateClient("ExtendedTimeoutClient");

        JsonSerializerOptions options = JsonSerializerOptions.Default;

        JsonNode schema = options.GetJsonSchemaAsNode(typeof(OllamaSummaryResponse));
        schema["type"] = "object";

        var partialSummaries = new List<string>();

        foreach (var fileChunk in fileChunks)
        {
            var chunkRequest = new OllamaSummaryRequest
            {
                Model = "llama3.2:1b",
                Prompt = $"""
                        Write a concise but complete summary of 4 to 6 sentences.

                        Do not return only a title, topic, or single-word answer.

                        Section to summarize:
                        {fileChunk.Chunk}
                        """,
                Format = schema,
                Stream = false
            };

            
            string json = JsonSerializer.Serialize(chunkRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var chunkResponse = await client.PostAsJsonAsync($"{ollamaBaseUrl}/api/generate",
            chunkRequest,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var chunkOllamaResult = await chunkResponse.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

            var chunkSummary = JsonSerializer.Deserialize<OllamaSummaryResponse>(
                chunkOllamaResult!.Response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            partialSummaries.Add(chunkSummary!.Summary);
        }

        var combinedSummaries = string.Join("\n\n", partialSummaries);

        var request = new OllamaSummaryRequest
        {
            Model = "llama3.2:1b",
            Prompt = $"""
                        Create one coherent summary of the complete document using the
                        section summaries below.

                        Remove repetition, preserve the document's main structure, and do not
                        introduce information that is absent from the summaries.

                        Section summaries:
                        {combinedSummaries}
                        """,
            Format = schema,
            Stream = false
        };

        var response = await client.PostAsJsonAsync($"{ollamaBaseUrl}/api/generate",
        request,
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

        var generalSummary = JsonSerializer.Deserialize<OllamaSummaryResponse>(
            ollamaResult!.Response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return generalSummary!.Summary;
    }

    public async IAsyncEnumerable<OllamaChatResponse> ChatAsync(string message, Guid documentId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chunkTokenBudget = (int)(128000 * 0.8);

        // generate embedding from question
        var queryEmbedding = await textExtractorService.GetEmbeddingForPrompt(message);

        // retrieve top k chunks
        var textChunks = await chunkRepository.GetRelevantChunksForEmbeddingForDocument(queryEmbedding, chunkTokenBudget, documentId);

        var request = new OllamaChatRequest
        {
            Model = "llama3.2:1b",
            System = $"You are a helpful assistant. Use only the following context to answer the user's question. If the answer isn't in the context, say so: \n\n {string.Join("\n", textChunks)}",
            Prompt = message,
            Stream = true
        };

        var client = clientFactory.CreateClient();

        using var httpRequest = new HttpRequestMessage(
        HttpMethod.Post,
        $"{ollamaBaseUrl}/api/generate")
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