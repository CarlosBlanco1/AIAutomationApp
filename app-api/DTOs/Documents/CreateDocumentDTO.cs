using System.ComponentModel.DataAnnotations;

public class CreateDocumentDTO
{
    [Required]
    [NotEmptyGuid(ErrorMessage = "Workspace Id is required")]
    public Guid WorkspaceId { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FileName { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxFileSize]
    [AllowedExtensions]
    public IFormFile File {get; set;} = null!;
}