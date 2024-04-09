namespace MultiTermSearch.Classes;

public class LineResult
{
    public int LineNumber { get; set; }
    public string Line { get; set; } = string.Empty;
    public List<TermResult> TermResults { get; } = new List<TermResult>();

    public void AddTermResult(TermResult termResult)
    {
        TermResults.Add(termResult);
        if (!FoundTerms.Contains(termResult.Term))
            FoundTerms.Add(termResult.Term);
    }

    public int MatchCount => TermResults.Sum(t => t.IndexOfMatches.Length);
    public List<string> FoundTerms { get; } = new List<string>();
}
