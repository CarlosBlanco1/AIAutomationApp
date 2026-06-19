using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CreateUserDTO
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string LastName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [StringLength(128, MinimumLength = 12)]
    [Password]
    public string Password { get; set; } = null!;

}