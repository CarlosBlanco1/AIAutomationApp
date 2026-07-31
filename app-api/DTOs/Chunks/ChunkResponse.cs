using System.Text.Json.Serialization;

public class ChunkResponse
{
    public int Index {get; set;}
    public string Chunk {get; set;} = null!;
    public float[] Vector {get; set;} = null!;

    [JsonPropertyName("token_size")]
    public int TokenSize {get; set;}
}