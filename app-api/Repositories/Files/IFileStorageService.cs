public interface IFileStorageService
{
    Task<UploadFileResult> UploadAsync(IFormFile file, string objectKey);
    Task DeleteAsync(string objectKey);
    Task<string> CreateDownloadUrlAsync(string objectKey);
}