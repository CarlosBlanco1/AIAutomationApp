public class DocumentDTO
{
    public Guid DocumentId { get; set; }

    public Guid WorkspaceId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileText { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}