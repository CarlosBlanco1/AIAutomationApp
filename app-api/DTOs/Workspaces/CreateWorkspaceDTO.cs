public class CreateWorkspaceDTO
{
    public string WorkspaceName { get; set; } = null!;

    public Guid OwnerId { get; set; }
}