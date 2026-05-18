using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class AutomationLog
{
    public int AutomationLogId { get; set; }

    public int? AutomationId { get; set; }

    public string LogStatus { get; set; } = null!;

    public string LogMessage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Automation? Automation { get; set; }
}
