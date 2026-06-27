using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

public class R2StorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly IAmazonS3 _s3Client;

    public R2StorageService(IConfiguration configuration, IAmazonS3 s3Client)
    {
        _configuration = configuration;
        _s3Client = s3Client;
    }
    public async Task<string> CreateDownloadUrlAsync(string objectKey)
    {
        // AWSConfigsS3.UseSignatureVersion4 = true;

        var serviceUrl = _configuration["SERVICE_URL"];
        var _accessKey = _configuration["ACESS_KEY"];
        var _secretKey = _configuration["SECRET_KEY"];
        var _bucketName = _configuration["BUCKET_NAME"];

        var config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
            // SignatureVersion = "v4",
            ForcePathStyle = true
        };

        using var s3Client = new AmazonS3Client(_accessKey, _secretKey, config);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        return s3Client.GetPreSignedURL(request);
    }

    public Task DeleteAsync(string objectKey)
    {
        throw new NotImplementedException();
    }

    public async Task<UploadFileResult> UploadAsync(IFormFile file, string objectKey)
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
                BucketName = _configuration["BUCKET_NAME"],
                Key = objectKey,
                InputStream = stream,
                ContentType = file.ContentType,

                DisablePayloadSigning = true
            };

            PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);

            return UploadFileResult.Success(response.ETag);
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