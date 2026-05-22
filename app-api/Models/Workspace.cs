using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class Workspace
{
    public Guid WorkspaceId { get; set; }

    public string WorkspaceName { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public virtual ICollection<Automation> Automations { get; set; } = new List<Automation>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User Owner { get; set; } = null!;
}
