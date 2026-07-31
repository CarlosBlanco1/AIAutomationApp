using app_api.Models;
using Pgvector;

public class Chunk
{
    public Guid ChunkId {get; set;}
    public Guid DocumentId {get; set;}
    public int ChunkIndex {get; set;}
    public string ChunkText {get; set;} = null!;
    public Vector Embedding {get; set;} = null!;
    public int TokenSize {get; set;}
    public virtual Document Document {get; set;} = null!;
}