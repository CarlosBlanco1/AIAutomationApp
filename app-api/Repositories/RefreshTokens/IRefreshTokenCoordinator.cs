public interface IRefreshTokenCoordinator
{
    Task<RefreshTokenIssueResult?> CreateOrUpdateRefreshTokenForUser(Guid userId, CancellationToken cancellationToken = default);
    Task<RefreshTokenIssueResult> ValidateAndRotateRefreshToken(string tokenHash, CancellationToken cancellationToken = default);
    Task ClearUserRefreshToken(Guid userId, CancellationToken cancellationToken);
}