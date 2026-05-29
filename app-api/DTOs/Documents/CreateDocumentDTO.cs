using System.ComponentModel.DataAnnotations;

public class CreateDocumentDTO
{
    [Required]
    [NotEmptyGuid(ErrorMessage = "Workspace Id is required")]
    public Guid WorkspaceId { get; set; }
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string FileName { get; set; } = null!;
    [Required]
    [StringLength(2048, MinimumLength = 2)]
    public string FilePath { get; set; } = null!;
    [Required]
    public string FileText { get; set; } = null!;
    [Required]
    [StringLength(2000, MinimumLength = 2)]
    public string Summary { get; set; } = null!;
}