using Amazon.S3.Model;

public interface IFileStorageService
{
    Task<UploadFileResult> UploadAsync(IFormFile file, string objectKey, CancellationToken cancellationToken);
    Task<string> DeleteAsync(string objectKey);
    Task<string> CreateDownloadUrlAsync(string objectKey);
    Task<GetObjectResponse> GetFileAsync(string objectKey, CancellationToken cancellationToken);
}