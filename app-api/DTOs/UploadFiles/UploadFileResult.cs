public class UploadFileResult
{
    public bool Succeeded {get; init;}
    public string Error {get; init;} = null!;
    public string ETag {get; init;} = null!;

    public static UploadFileResult Success(string etag) =>
        new() { Succeeded = true, ETag = etag };

    public static UploadFileResult Fail(string error) =>
        new() { Succeeded = false, Error = error };

}