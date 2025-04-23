using System.Runtime.CompilerServices;

namespace MultiTermSearch.Classes;

public class FileResult
{
    public string FilePath { get; set; } = null!;
    public string? ErrorMessage { get; set; } = null;
    public List<LineResult> LineResults { get; set; } = new List<LineResult>();
    public bool HasResult => LineResults.Any();

    public FileResult(string filePath) { FilePath = filePath; }
    public FileResult(string filePath, string errorMessage)
    {
        FilePath = filePath;
        ErrorMessage = errorMessage;
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
    public void AddLineResults(List<LineResult> lineResults)
    {
        LineResults.AddRange(lineResults);
        var foundTermsInBatch = lineResults
            .SelectMany(x => x.FoundTerms)
            .Distinct()
            .ToList();

        foreach (var foundTerm in foundTermsInBatch)
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
    public List<string> FoundTerms { get; private set; } = new List<string>();

    public int MatchCount => LineResults.Sum(r=> r.MatchCount);


    public static FileResult Ok(string filePath, List<LineResult> results)
    {
        return new FileResult(filePath);
    }

    public static FileResult Error(string filePath, string error)
    {
        return new FileResult(filePath, error);
    }
}
