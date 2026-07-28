public class OllamaChatRequest
{
    public string Model {get; set;} = string.Empty;
    public string Prompt {get; set;} = string.Empty;
    public bool Stream {get; set;} = true;
}
