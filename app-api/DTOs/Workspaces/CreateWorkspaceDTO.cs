using System.ComponentModel.DataAnnotations;

public class CreateWorkspaceDTO
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string WorkspaceName { get; set; } = null!;
    [Required]
    [NotEmptyGuid(ErrorMessage = "Owner Id is required")]
    public Guid OwnerId { get; set; }
}