using System.Text.RegularExpressions;

namespace MultiTermSearch.Classes;

public class SearchRegex : Regex
{
    public string SearchTerm { get; set; } = string.Empty;

    public SearchRegex(string searchPattern, string baseSearchTerm, RegexOptions options) : base(searchPattern, options) 
    { 
        SearchTerm = baseSearchTerm;
    }
}
