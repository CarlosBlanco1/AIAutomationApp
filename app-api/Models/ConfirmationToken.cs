using System;
using System.Collections.Generic;

namespace app_api.Models;

public partial class ConfirmationToken
{
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public required TokenPurpose Purpose { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly ExpiresAt { get; set; }
    
    public virtual User User { get; set; } = null!;
}

public enum TokenPurpose
{
    EmailConfirm,
    PasswordChange
}
