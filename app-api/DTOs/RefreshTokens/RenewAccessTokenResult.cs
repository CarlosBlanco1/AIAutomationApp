public record RenewAccessTokenResult
(
    string RawRefreshToken,
    DateTime ExpiresAt,
    Guid UserId
);