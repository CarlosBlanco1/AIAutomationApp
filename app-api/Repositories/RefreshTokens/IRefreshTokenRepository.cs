public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task RemoveTokenForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpsertTokenForUserAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

}