using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

public class OllamaSummaryService : ISummaryService
{
    private readonly IHttpClientFactory clientFactory;

    public OllamaSummaryService(IHttpClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }
    public async Task<string> GenerateSummary(string fileText)
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

        var response = await client.PostAsJsonAsync("http://workspaceai-ollama-svc:11434/api/generate", request);

        var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

        var summary = JsonSerializer.Deserialize<OllamaSummaryResponse>(
            ollamaResult!.Response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return summary.Summary;
    }
}