using app_api.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;

public class SQLChunkRepository : IChunkRepository
{
    private readonly MydbContext dbContext;

    public SQLChunkRepository(MydbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<List<Chunk>> CreateChunksAsync(List<Chunk> chunks)
    {
        await dbContext.Chunks.AddRangeAsync(chunks);
        await dbContext.SaveChangesAsync();

        return chunks;
    }

    public async Task<List<string>> GetRelevantChunksForEmbeddingForDocument(Vector queryEmbedding, int tokenBudget, Guid documentId)
    {
        if(tokenBudget <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenBudget), "Token budget must be greater than 0");
        }

        var chunksAndIndeces = await dbContext.Database.SqlQuery<ChunkSearchResult>(
            $"""
            WITH candidates AS (
                SELECT
                    chunk_text AS "Text",
                    chunk_index AS "Index",
                    token_size AS "TokenSize",
                    "Embedding" <=> {queryEmbedding} AS "Distance"
                FROM chunks
                WHERE "DocumentId" = {documentId}
                ORDER BY "Embedding" <=> {queryEmbedding}
                LIMIT 50
            ),
            running AS (
                SELECT
                    *,
                    SUM("TokenSize") OVER (
                        ORDER BY "Distance"
                    ) AS "CumulativeTokens"
                FROM candidates
            )
            SELECT
                "Text",
                "Index"            
            FROM running
            WHERE "CumulativeTokens" <= {tokenBudget}
            ORDER BY "Distance"
            """)
        .ToListAsync();

        return chunksAndIndeces.OrderBy(c => c.Index).Select(c => c.Text).ToList();
    }

    public async Task<List<Chunk>> RetrieveChunksByDocumentIdAsync(Guid documentId)
    {
        return await dbContext.Chunks
        .Where(c => c.DocumentId == documentId)
        .Select(c => new Chunk
        {
            ChunkId = c.ChunkId,
            DocumentId = c.DocumentId,
            ChunkIndex = c.ChunkIndex,
            ChunkText = c.ChunkText,
            Embedding = c.Embedding
        })
        .ToListAsync();
    }
}