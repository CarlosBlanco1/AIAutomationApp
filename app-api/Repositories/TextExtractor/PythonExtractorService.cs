using System.Text.Json;
using Amazon.Util.Internal;

class PythonExtractorService : ITextExtractorService
{
    private readonly IHttpClientFactory httpClientFactory;

    public PythonExtractorService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }
    public async Task<TextResponse> GetTextExtractedAsync(IFormFile file, string fileName)
    {
        var client = httpClientFactory.CreateClient();

        var fileStream = file.OpenReadStream();

        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(fileStream), "file", fileName);

        var response = await client.PostAsync(
            "http://myapp-text-server:8000/text-extractor",
            content);

        return await response.Content.ReadFromJsonAsync<TextResponse>();
    }
}