public class WorkspaceDTO
{
    public Guid WorkspaceId { get; set; }

    public string WorkspaceName { get; set; } = null!;

    public Guid OwnerId { get; set; }
}