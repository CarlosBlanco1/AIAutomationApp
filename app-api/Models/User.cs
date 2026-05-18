using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
