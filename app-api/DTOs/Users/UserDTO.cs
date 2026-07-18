public class UserDTO
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool EmailConfirmed  { get; set; }
    
    public DateOnly CreatedAt { get; set; }
}