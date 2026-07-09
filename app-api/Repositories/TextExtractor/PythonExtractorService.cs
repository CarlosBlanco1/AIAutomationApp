using System.Text.Json;
using Amazon.Util.Internal;

class PythonExtractorService : ITextExtractorService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    public PythonExtractorService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }
    public async Task<TextResponse> GetTextExtractedAsync(IFormFile file, string fileName)
    {
        var client = httpClientFactory.CreateClient();

        var fileStream = file.OpenReadStream();

        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(fileStream), "file", fileName);

        var textExtractorUrl = configuration["TEXT_EXTRACTOR_URL"];

        var response = await client.PostAsync(
            $"{textExtractorUrl}/text-extractor",
            content);

        return await response.Content.ReadFromJsonAsync<TextResponse>();
    }
}