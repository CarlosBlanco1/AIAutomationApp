public interface IFileStorageService
{
    Task<UploadFileResult> UploadAsync(IFormFile file, string objectKey);
    Task<string> DeleteAsync(string objectKey);
    Task<string> CreateDownloadUrlAsync(string objectKey);
}