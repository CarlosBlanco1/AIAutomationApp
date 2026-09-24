using System.ComponentModel.DataAnnotations;

public class CreateWorkspaceDTO
{
    [Required]
    public string RequestKey { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string WorkspaceName { get; set; } = null!;
}