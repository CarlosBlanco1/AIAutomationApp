public class CreateAutomationLogDTO
{
    public Guid AutomationId { get; set; }

    public string LogStatus { get; set; } = null!;

    public string LogMessage { get; set; } = null!;
}