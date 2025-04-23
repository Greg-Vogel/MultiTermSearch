namespace MultiTermSearch.Classes;

public class LineResult
{
    public int LineNumber { get; set; }
    public string Line { get; set; } = string.Empty;
    public Dictionary<string, List<int>> TermResults { get; } = new Dictionary<string, List<int>>();

    public void AddTermResult(string term, int indexOfMatch)
    {
        if (TermResults.TryGetValue(term, out var indexList))
            indexList.Add(indexOfMatch);
        else
            TermResults.Add(term, new List<int> {indexOfMatch});
    }
    public void AddTermResults(string term, List<int> indexesOfMatches)
    {
        if (TermResults.TryGetValue(term, out var indexList))
            indexList.AddRange(indexesOfMatches);
        else
            TermResults.Add(term, indexesOfMatches);
    }

    public int MatchCount => TermResults.Values.Sum(v => v.Count);
    public List<string> FoundTerms => TermResults.Keys.ToList();
}
