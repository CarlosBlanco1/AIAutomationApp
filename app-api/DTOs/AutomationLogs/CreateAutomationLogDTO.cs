using System.ComponentModel.DataAnnotations;

public class CreateAutomationLogDTO
{
    [Required]
    [NotEmptyGuid(ErrorMessage = "Automation Id is required")]
    public Guid AutomationId { get; set; }
    [Required]
    [StringLength(7, MinimumLength = 6)]
    public string LogStatus { get; set; } = null!;
    [Required]
    public string LogMessage { get; set; } = null!;
}