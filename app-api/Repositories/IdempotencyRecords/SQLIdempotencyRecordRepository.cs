using app_api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class SQLIdempotencyRecordRepository : IIdempotencyRecordRepository
{
    private readonly MydbContext context;
    private readonly ILogger<SQLIdempotencyRecordRepository> logger;

    public SQLIdempotencyRecordRepository(MydbContext context, ILogger<SQLIdempotencyRecordRepository> logger)
    {
        this.context = context;
        this.logger = logger;
    }
    public async Task<(IdempotencyRecord, bool)> CreateIdempotencyRecordAsync(Guid userId, IdempotencyRecordOperation operation, string clientKey, string requestBodyHash, CancellationToken cancellationToken = default)
    {
        var newIdempotencyRecord = new IdempotencyRecord
        {
            UserId = userId,
            Operation = operation,
            ClientKey = clientKey,
            RequestBodyHash = requestBodyHash,
            Status = IdempotencyRecordStatus.Processing,
            ExpirationDate = DateTime.UtcNow.AddHours(24)
        };

        try
        {
            newIdempotencyRecord = context.IdempotencyRecords.Add(newIdempotencyRecord).Entity;
            await context.SaveChangesAsync(cancellationToken);

            return (newIdempotencyRecord, true);
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return (newIdempotencyRecord, false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error ocurred while creating new Idempotency Record");
            throw;
        }
    }

    public async Task DeleteExpiredIdempotencyRecordsAsync(CancellationToken cancellationToken)
    {
        await context.IdempotencyRecords.Where(ir => ir.ExpirationDate < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteFailedIdempotencyRecordAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recordToDelete = await context.IdempotencyRecords.FirstOrDefaultAsync(ir => ir.Id == id);

        context.IdempotencyRecords.Remove(recordToDelete!);
        await context.SaveChangesAsync();
    }

    public Task<IdempotencyRecord?> FetchIdempotencyRecordByKeyAsync(Guid userId, IdempotencyRecordOperation operation, string clientKey, CancellationToken cancellationToken = default)
    {
        return context.IdempotencyRecords.SingleOrDefaultAsync(ir => 
        ir.UserId == userId &&
        ir.Operation == operation &&
        ir.ClientKey == clientKey,
        cancellationToken);
    }

    public async Task MarkIdempotencyRecordAsCompleteAsync(Guid id, int responseStatusCode, string responseBody, CancellationToken cancellationToken = default)
    {
        var recordToUpdate = await context.IdempotencyRecords.SingleOrDefaultAsync(ir => ir.Id == id, cancellationToken);

        if(recordToUpdate == null)
        {
            throw new Exception("No Idempotency Record exists for given Id");
        }

        recordToUpdate.ResponseStatusCode = responseStatusCode;
        recordToUpdate.ResponseBody = responseBody;
        recordToUpdate.Status = IdempotencyRecordStatus.Completed;

        await context.SaveChangesAsync(cancellationToken);
    }
}