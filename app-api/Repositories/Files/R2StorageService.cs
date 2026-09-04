using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

public class R2StorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<R2StorageService> _logger;
    private readonly string _bucketName;

    public R2StorageService(IConfiguration configuration, IAmazonS3 s3Client, ILogger<R2StorageService> logger)
    {
        _configuration = configuration;
        _s3Client = s3Client;
        _logger = logger;
        _bucketName =  _configuration["BUCKET_NAME"]!;
    }
    public async Task<string> CreateDownloadUrlAsync(string objectKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        return _s3Client.GetPreSignedURL(request);
    }

    public async Task<string> DeleteAsync(string objectKey)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = objectKey
        };

        try
        {
            DeleteObjectResponse response = await _s3Client.DeleteObjectAsync(deleteRequest);
            if(response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
            {     
                return "Successful deletion!";
            }
            else
            {
                throw new Exception("An error ocurred while deleting the file!");
            }
        }
        catch (AmazonS3Exception e)
        {
            return $"Error encountered on server. Message:'{e.Message}' when deleting an object";
        }
        catch (Exception e)
        {
            return $"Unknown encountered on server. Message:'{e.Message}' when deleting an object";
        }
    }

    public async Task<UploadFileResult> UploadAsync(IFormFile file, string objectKey, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return UploadFileResult.Fail("No file uploaded!");
        }

        try
        {
            using var stream = file.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                InputStream = stream,
                ContentType = file.ContentType,
                DisablePayloadSigning = true
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);

            return UploadFileResult.Success(response.ETag);
        }
        catch(OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Object upload cancelled for object with key {objectKey}", objectKey);
            throw;
        }
        catch (AmazonS3Exception ex)
        {
            return UploadFileResult.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            return UploadFileResult.Fail(ex.Message);
        }
    }
}