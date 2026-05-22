public class CreateAutomationDTO
{
    public Guid WorkspaceId { get; set; }

    public string AutomationName { get; set; } = null!;

    public string TriggerType { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string WebhookUrl { get; set; } = null!;

    public bool IsActive { get; set; }
}