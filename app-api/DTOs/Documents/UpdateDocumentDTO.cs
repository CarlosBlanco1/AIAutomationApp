using System.ComponentModel.DataAnnotations;

public class UpdateDocumentDTO
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string FileName { get; set; } = null!;

}