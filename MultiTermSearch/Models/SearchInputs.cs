using System.Text.Json.Serialization;

namespace MultiTermSearch.Models;

internal class SearchInputs
{
    public string Path { get; set; } = null!;
    public bool IncludeSubDir { get; set; }
    public string[] SearchTerms { get; set; } = null!;
    public bool Options_MatchWholeWord { get; set; }

    private bool _optionIgnoreCase = false;
    public bool Options_IgnoreCase
    {
        get { return _optionIgnoreCase; }
        set
        {
            _optionIgnoreCase = value;
            StringComparison = _optionIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }
    }
    public StringComparison StringComparison { get; private set; }
    public bool Filters_LineContainsAll { get; set; }
    public bool Filters_FileContainsAll { get; set; }
    public ESearchTargets Target { get; set; }

    private string[] _includeFileTypes = [];
    public string[] IncludeTypes 
    { 
        get {  return _includeFileTypes; } 
        set { _includeFileTypes = value; ScanAllFileTypes = _includeFileTypes.Contains(".*"); } 
    }
    public bool ScanAllFileTypes { get; private set; }

    public byte SearcherThreadCount { get; set; } = 1;

    public enum ESearchTargets
    {
        Both = 0,
        FileNames = 1,
        FileContents = 2,
    }
}
