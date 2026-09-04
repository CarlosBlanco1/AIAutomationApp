using System.Text.Json;
using Amazon.Util.Internal;
using Pgvector;

class PythonExtractorService : ITextExtractorService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;
    private readonly ILogger<PythonExtractorService> logger;

    public PythonExtractorService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PythonExtractorService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
        this.logger = logger;
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

    public async Task<List<ChunkResponse>> GetTextEmbeddedChunksAsync(IFormFile file, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ExtendedTimeoutClient");

            using var fileStream = file.OpenReadStream();
            using var fileContent = new StreamContent(fileStream);
            using var content = new MultipartFormDataContent();

            content.Add(fileContent, "file", fileName);

            var textExtractorUrl = configuration["TEXT_EXTRACTOR_URL"];

            var response = await client.PostAsync(
                $"{textExtractorUrl}/generate-embedded-chunks",
                content,
                cancellationToken);

            return (await response.Content.ReadFromJsonAsync<List<ChunkResponse>>(cancellationToken))!;
        }
        catch(OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Retrieving embedded chunks operation cancelled for file '{fileName}'", fileName);
            throw;
        }
    }
}