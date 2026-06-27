public class DocumentDTO
{
    public Guid DocumentId { get; set; }

    public string WorkspaceName { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string BlobKey { get; set; } = null!;

    public long FileSizeBytes {get; set;} 
    
    public string Description {get; set;} = null!;

    public string FileText { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}