using Amazon.Util.Internal;

public interface ITextExtractorService
{
    Task<TextResponse> GetTextExtractedAsync(IFormFile file, string fileName);
}