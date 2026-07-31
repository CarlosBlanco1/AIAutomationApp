using System.Text.Json;
using Amazon.Util.Internal;
using Pgvector;

class PythonExtractorService : ITextExtractorService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    public PythonExtractorService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    public async Task<Vector> GetEmbeddingForPrompt(string prompt)
    {
        var client = httpClientFactory.CreateClient("ExtendedTimeoutClient");
        var textExtractorUrl = configuration["TEXT_EXTRACTOR_URL"];

        var response = await client.PostAsJsonAsync($"{textExtractorUrl}/generate-prompt-embedding", new { prompt });

        var unformattedEmbedding = await response.Content.ReadFromJsonAsync<float[]>();

        if(unformattedEmbedding is null)
        {
            throw new InvalidOperationException("the API returned no embedding!");
        }

        return new Vector(unformattedEmbedding);
    }

    public async Task<List<ChunkResponse>> GetTextEmbeddedChunksAsync(IFormFile file, string fileName)
    {
        var client = httpClientFactory.CreateClient("ExtendedTimeoutClient");

        using var fileStream = file.OpenReadStream();

        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(fileStream), "file", fileName);

        var textExtractorUrl = configuration["TEXT_EXTRACTOR_URL"];

        var response = await client.PostAsync(
            $"{textExtractorUrl}/generate-embedded-chunks",
            content);

        return (await response.Content.ReadFromJsonAsync<List<ChunkResponse>>())!;
    }
}