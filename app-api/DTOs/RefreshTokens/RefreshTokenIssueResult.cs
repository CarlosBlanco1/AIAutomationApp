public record RefreshTokenIssueResult
(
    string RawRefreshToken,
    DateTime ExpiresAt
);