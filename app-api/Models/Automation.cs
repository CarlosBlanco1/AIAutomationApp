using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class Automation
{
    public int AutomationId { get; set; }

    public int? WorkspaceId { get; set; }

    public string AutomationName { get; set; } = null!;

    public string TriggerType { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string WebhookUrl { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<AutomationLog> AutomationLogs { get; set; } = new List<AutomationLog>();

    public virtual Workspace? Workspace { get; set; }
}
