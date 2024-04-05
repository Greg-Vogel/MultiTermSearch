namespace MultiTermSearch.Models;

public class FileResult
{
    public string FilePath { get; set; } = null!;
    public List<LineResult> LineResults { get; set; } = new List<LineResult>();

    public FileResult(string filePath) { FilePath = filePath; }

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
