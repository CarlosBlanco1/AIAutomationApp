using System.Text.Json.Nodes;

public class OllamaSummaryRequest
{
    public string Model { get; set; } = null!;
    public string Prompt { get; set; } = null!;
    public JsonNode Format {get; set;} = null!;
    public bool Stream {get; set;} = false;
}