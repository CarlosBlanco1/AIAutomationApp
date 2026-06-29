public interface ISummaryService
{
    Task<string> GenerateSummary(string fileText);
}