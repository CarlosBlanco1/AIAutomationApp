using app_api.Models;
using Microsoft.EntityFrameworkCore;

public class SQLRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MydbContext context;

    public SQLRefreshTokenRepository(MydbContext context)
    {
        this.context = context;
    }
    public async Task<RefreshToken?> GetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens.SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
    }

    public async Task RemoveTokenForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokenToDelete = await context.RefreshTokens.SingleOrDefaultAsync(rt => rt.UserId == userId, cancellationToken);

        if (tokenToDelete is null)
        {
            return;
        }

        context.RefreshTokens.Remove(tokenToDelete);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertTokenForUserAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenToUpdate = await context.RefreshTokens.SingleOrDefaultAsync(rt => rt.UserId == refreshToken.UserId, cancellationToken);

        if (tokenToUpdate is null)
        {
            context.RefreshTokens.Add(refreshToken);
        }
        else
        {
            tokenToUpdate.TokenHash = refreshToken.TokenHash;
            tokenToUpdate.ExpiresAt = refreshToken.ExpiresAt;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}