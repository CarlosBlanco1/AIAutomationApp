using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public int? WorkspaceId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileText { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Workspace? Workspace { get; set; }
}
