using System.ComponentModel.DataAnnotations;

public class UpdateDocumentDTO
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Description { get; set; } = null!;
}