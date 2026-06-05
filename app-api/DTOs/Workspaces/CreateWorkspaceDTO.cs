using System.ComponentModel.DataAnnotations;

public class CreateWorkspaceDTO
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string WorkspaceName { get; set; } = null!;
}