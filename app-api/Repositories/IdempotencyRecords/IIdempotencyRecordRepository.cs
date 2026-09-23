public interface IIdempotencyRecordRepository
{
    Task<(IdempotencyRecord, bool)> CreateIdempotencyRecordAsync(Guid userId, IdempotencyRecordOperation operation, string clientKey, string requestBodyHash, CancellationToken cancellationToken = default);
    Task<IdempotencyRecord?> FetchIdempotencyRecordByKeyAsync(Guid userId, IdempotencyRecordOperation operation, string clientKey, CancellationToken cancellationToken = default);
    Task MarkIdempotencyRecordAsCompleteAsync(Guid id, int responseStatusCode, string responseBody, CancellationToken cancellationToken = default);
    Task DeleteExpiredIdempotencyRecordsAsync(CancellationToken cancellationToken = default);
    Task DeleteFailedIdempotencyRecordAsync(Guid id, CancellationToken cancellationToken = default);
}