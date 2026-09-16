using app_api.Models;

public class RefreshToken
{
    public Guid TokenId {get; set;}
    public Guid UserId {get; set;}
    public string TokenHash {get; set;} = null!;
    public DateTime CreatedAt {get; set;}
    public DateTime ExpiresAt {get; set;}
    public virtual User TokenUser { get; set; } = null!;
}