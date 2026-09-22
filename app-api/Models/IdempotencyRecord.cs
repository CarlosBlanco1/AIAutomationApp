using app_api.Models;

public class IdempotencyRecord
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}
    public IdempotencyRecordOperation Operation {get; set;}
    public string ClientKey {get; set;} = null!;
    public string RequestBodyHash {get; set;} = null!;
    public int? ResponseStatusCode {get; set;}
    public string? ResponseBody {get; set;}
    public IdempotencyRecordStatus Status {get; set;} = IdempotencyRecordStatus.Processing;
    public DateTime ExpirationDate {get; set;}
    public virtual User Originator { get; set; } = null!;
}