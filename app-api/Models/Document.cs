using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class Document
{
    public Guid DocumentId { get; set; }

    public Guid WorkspaceId { get; set; }

    public string FileName { get; set; } = null!;

    public string BlobKey { get; set; } = null!;

    public long FileSizeBytes {get; set;} 

    public string Description {get; set;} = null!;

    public string FileText { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Workspace Workspace { get; set; } = null!;
}
