using System.ComponentModel.DataAnnotations;

public class UpdateUserDTO
{
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string LastName { get; set; } = null!;
    
}