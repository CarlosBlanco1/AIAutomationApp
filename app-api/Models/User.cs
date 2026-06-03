using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace app_api.Models;

public partial class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
