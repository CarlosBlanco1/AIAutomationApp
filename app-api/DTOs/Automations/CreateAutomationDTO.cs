using System.ComponentModel.DataAnnotations;

public class CreateAutomationDTO
{
    [Required]
    [NotEmptyGuid(ErrorMessage = "Workspace Id is required")]
    public Guid WorkspaceId { get; set; }
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string AutomationName { get; set; } = null!;
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string TriggerType { get; set; } = null!;
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string ActionType { get; set; } = null!;
    [Required]
    [StringLength(2048, MinimumLength = 2)]
    public string WebhookUrl { get; set; } = null!;
    [Required]
    public bool IsActive { get; set; }
}