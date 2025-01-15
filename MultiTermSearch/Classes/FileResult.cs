namespace MultiTermSearch.Classes;

public class FileResult
{
    public string FilePath { get; set; } = null!;
    public string? Error { get; set; } = null;
    public List<LineResult> LineResults { get; set; } = new List<LineResult>();
    public bool HasResult => LineResults.Any();

    public FileResult(string filePath) { FilePath = filePath; }
    public FileResult(string filePath, string errorMessage)
    {
        FilePath = filePath;
        Error = errorMessage;
    }

    public void AddLineResult(LineResult lineResult)
    {
        LineResults.Add(lineResult);
        foreach (var foundTerm in lineResult.FoundTerms)
        {
            if (!FoundTerms.Contains(foundTerm))
                FoundTerms.Add(foundTerm);
        }
    }
    public string FileName
    {
        get
        {
            if (FilePath is null) 
                return string.Empty;
            return FilePath.Substring(FilePath.LastIndexOf('\\') + 1);
        }
    }
    public List<string> FoundTerms { get; } = new List<string>();
    public int MatchCount => LineResults.Sum(r=> r.MatchCount);
}
