public class AutomationLogDTO
{
    public Guid AutomationLogId { get; set; }

    public Guid AutomationId { get; set; }

    public string LogStatus { get; set; } = null!;

    public string LogMessage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}