using System.ComponentModel.DataAnnotations;

public class LoginRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [RegularExpression(
    @"^(?=.*[A-Za-z])(?=.*\d).+$",
    ErrorMessage = "Password must contain at least one letter and one number."
    )]
    public string Password { get; set; } = null!;
}