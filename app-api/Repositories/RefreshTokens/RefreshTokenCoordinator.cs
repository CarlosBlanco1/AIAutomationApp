using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

public class RefreshTokenCoordinator : IRefreshTokenCoordinator
{
    private readonly IRefreshTokenRepository refreshTokenRepository;
    private readonly IUserRepository userRepository;

    public RefreshTokenCoordinator(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
    {
        this.refreshTokenRepository = refreshTokenRepository;
        this.userRepository = userRepository;
    }

    public async Task<RefreshTokenIssueResult?> CreateOrUpdateRefreshTokenForUser(Guid userId, CancellationToken cancellationToken = default)
    {
        if(await userRepository.GetUserByIdAsync(userId) is null)
        {
            throw new Exception("Refresh token update/create failed, user doesn't exist");
        }

        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var bytes = Encoding.UTF8.GetBytes(rawRefreshToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = Convert.ToBase64String(SHA256.HashData(bytes)),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.UpsertTokenForUserAsync(refreshToken, cancellationToken);

        return new RefreshTokenIssueResult(rawRefreshToken, refreshToken.ExpiresAt);
    }

    public async Task<RefreshTokenIssueResult> ValidateAndRotateRefreshToken(string rawRefreshToken, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(rawRefreshToken);
        var tokenHash = Convert.ToBase64String(SHA256.HashData(bytes));

        var tokenToValidate = await refreshTokenRepository.GetTokenByHashAsync(tokenHash, cancellationToken);

        if(tokenToValidate is null)
        {
            throw new Exception("Couldn't find a refresh token with given hash");
        }

        if(DateTime.Compare(tokenToValidate.ExpiresAt, DateTime.UtcNow) < 0)
        {
            throw new Exception("Refresh Token has already expired");
        }

        var newRawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newBytes = Encoding.UTF8.GetBytes(newRawRefreshToken);

        var newRefreshToken = new RefreshToken
        {
            UserId = tokenToValidate.UserId,
            TokenHash = Convert.ToBase64String(SHA256.HashData(newBytes)),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.UpsertTokenForUserAsync(newRefreshToken, cancellationToken);

        return new RefreshTokenIssueResult(newRawRefreshToken, newRefreshToken.ExpiresAt);
    }

    public async Task ClearUserRefreshToken(Guid userId, CancellationToken cancellationToken)
    {
        if(await userRepository.GetUserByIdAsync(userId) is null)
        {
            throw new Exception("Refresh token clearance failed, user doesn't exist");
        }

        await refreshTokenRepository.RemoveTokenForUserAsync(userId, cancellationToken);
    }
}