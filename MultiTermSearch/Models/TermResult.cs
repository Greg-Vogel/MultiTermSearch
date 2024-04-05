namespace MultiTermSearch.Models;

public class TermResult
{
    public string Term { get; set; } = string.Empty;
    public int[] IndexOfMatches { get; set; } = [];
}
