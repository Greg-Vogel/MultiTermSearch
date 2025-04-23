
namespace MultiTermSearch.Classes;

public class SearchInputs
{
    public string Path { get; set; } = null!;
    public bool IncludeSubDir { get; set; }
    public string[] SearchTerms { get; set; } = null!;
    public byte SearcherThreadCount { get; set; } = 1;


    #region O P T I O N S

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

    public bool Options_ExcludeLargeDirectories { get; set; }
    public bool Options_ExcludeBinaries { get; set; }

    #endregion


    #region M A T C H   F I L T E R S 

    public bool Filters_LineContainsAll { get; set; }
    public bool Filters_FileContainsAll { get; set; }
    public ESearchTargets Target { get; set; }

    #endregion


    #region I N C L U D E   F I L E   T Y P E S

    private string[] _includeFileTypes = [];
    public string[] IncludeTypes
    {
        get { return _includeFileTypes; }
        set { _includeFileTypes = value; ScanAllFileTypes = _includeFileTypes.Contains(".*"); }
    }
    public bool ScanAllFileTypes { get; private set; }

    public enum ESearchTargets
    {
        Both = 0,
        FileNames = 1,
        FileContents = 2,
    }

    #endregion
}
