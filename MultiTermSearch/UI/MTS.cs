using MultiTermSearch.Classes;
using MultiTermSearch.Events;
using MultiTermSearch.Helpers;
using MultiTermSearch.Properties;
using MultiTermSearch.SearchLogic;

namespace MultiTermSearch;

public partial class MTS : Form
{
    private SearchDriver _searcher = null!;

    public MTS()
    {
        InitializeComponent();
        InitializeSearcher();
        DoubleBuffered = true;

        SettingsHelper.LoadSettings();
        SetUIFields(SettingsHelper.CurrentSettings);
    }


    private void InitializeSearcher()
    {
        if (_searcher != null)
            return;
        _searcher = new SearchDriver();
        _searcher.FileListIdentifiedEvent += Searcher_FileListIdentifiedEvent;
        _searcher.FileProcessedEvent += Searcher_FileProcessedEvent;
        _searcher.SearchCompleteEvent += Searcher_SearchCompleteEvent;
        _searcher.FileSkppedEvent += Searcher_FileExcludedEvent;

    }
    private void SetUIFields(MtsSettings settings)
    {
        // Apply those last used values to the fields
        txtPath.Text = settings.Path;
        chkIncludeSubDir.Checked = settings.IncludeSubDir;
        chkWholeWord.Checked = settings.Options_MatchWholeWord;
        chkIgnoreCase.Checked = settings.Options_IgnoreCase;
        chkExcludeLargeDir.Checked = settings.Options_ExcludeLargeDirectories;
        chkFilterLineContains.Checked = settings.Filters_LineContainsAll;
        chkFilterFileContains.Checked = settings.Filters_FileContainsAll;
        rtFileTypes.Lines = settings.IncludeTypes;
        switch (settings.Target)
        {
            case SearchInputs.ESearchTargets.FileNames:
                radTargetFileNames.Checked = true;
                break;
            case SearchInputs.ESearchTargets.FileContents:
                radTargetContents.Checked = true;
                break;
            default:
                radTargetBoth.Checked = true;
                break;
        }
        cbThreads.SelectedItem = settings.MaxThreadCount.ToString();
        this.ActiveControl = rtSearchTerms;
    }

    private SearchInputs GetInputsFromUI()
    {
        return new SearchInputs()
        {
            Path = txtPath.Text,
            IncludeSubDir = chkIncludeSubDir.Checked,
            SearchTerms = rtSearchTerms.Lines.ToList().Where(l => !string.IsNullOrEmpty(l)).ToArray(),
            Options_IgnoreCase = chkIgnoreCase.Checked,
            Options_MatchWholeWord = chkWholeWord.Checked,
            Options_ExcludeLargeDirectories = chkExcludeLargeDir.Checked,
            Options_ExcludeBinaries = chkExcludeBinaries.Checked,
            Filters_LineContainsAll = chkFilterLineContains.Checked,
            Filters_FileContainsAll = chkFilterFileContains.Checked,
            IncludeTypes = rtFileTypes.Lines.ToList().Where(l => !string.IsNullOrWhiteSpace(l)).ToArray(),
            SearcherThreadCount = Convert.ToByte(cbThreads.SelectedItem),

            Target = radTargetBoth.Checked
                ? SearchInputs.ESearchTargets.Both
                : radTargetFileNames.Checked
                    ? SearchInputs.ESearchTargets.FileNames
                    : SearchInputs.ESearchTargets.FileContents
        };
    }

    private bool ValidSearchInputs(SearchInputs inputs)
    {
        var validationErrors = new List<string>();
        if (inputs.IncludeTypes is null || inputs.IncludeTypes.Length == 0)
        {
            validationErrors.Add("File type is required, if you want to search all file types use '.*'");
        }

        if (string.IsNullOrWhiteSpace(inputs.Path)
            || !Directory.Exists(inputs.Path))
        {
            validationErrors.Add("A valid folder path is required to start searching.");
        }

        if (inputs.SearchTerms is null || inputs.SearchTerms.Length == 0)
        {
            validationErrors.Add("A search term is required to start searching.");
        }

        if (inputs.SearchTerms!.Any(t=> t.Length < 3))
        {
            if (MessageBox.Show("Your search terms contains a term that is shorter than 3 characters.\nThis will greatly impact performance due to the potential number of results.\n\nAre you sure you want to continue?"
                    , "Confirm Open Ended Search?"
                    , MessageBoxButtons.YesNo)
                != DialogResult.Yes)
            {
                return false;
            }
        }

        if (validationErrors.Any())
        {
            string message = string.Empty;
            validationErrors.ForEach(err => message += $"{err}{Environment.NewLine}");
            MessageBox.Show(message, "Search Input Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    private void StartSearch()
    {
        // Clear previous results
        resultsControl1.ClearResults();

        // Get the search inputs
        var inputs = GetInputsFromUI();

        // Validate UI inputs to make sure they make sense before we save them off or start an open ended search
        if (!ValidSearchInputs(inputs))
        {
            btnSearch.Enabled = true;
            return;
        }

        // Save these inputs as the last used settings
        //    kicking it off in a separate task that is not waited because it isnt important and holdup the search
        Task.Run(() => SettingsHelper.SaveSettings(inputs));

        // Start the actual search process
        _searcher.StartSearchAsync(inputs);

        // Sets the result control to start expecting results
        //   This enables a limited redraw rate to prevent freezing the UI
        resultsControl1.SetSearchBegin(inputs.Path);

        // Now that the search is actually running
        //   switch the 'search' button into a 'cancel' one
        btnSearch.Text = "Cancel";
        btnSearch.Enabled = true;
    }


    private void btnBrowse_Click(object sender, EventArgs e)
    {
        var fbd = new FolderBrowserDialog()
        {
            InitialDirectory = Settings.Default.LastSettings,
            ShowNewFolderButton = false
        };
        if (fbd.ShowDialog() == DialogResult.OK)
        {
            txtPath.Text = fbd.SelectedPath;
        }
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
        btnSearch.Enabled = false;

        // If the searcher is already running... then this button click is to actually Cancel it
        if (_searcher is not null && _searcher.SearchInProgress)
        {
            _searcher.CancelSearchAsync();
            resultsControl1.SetSearchCancelling();
            return;
        }

        // The searcher was not running... so Start it
        StartSearch();
    }


    private void Searcher_FileListIdentifiedEvent(object? sender, FileListIdentifiedEventArgs e)
    {
        // notify the results of how many files there are so it can display the progress & status correctly
        resultsControl1.SetTotalFileCount(e.FileCount);
    }
    private void Searcher_FileProcessedEvent(object? sender, ItemAddedEventArgs e)
    {
        // if we actually got a match, count it and add it to the display
        resultsControl1.HandleResult(e.Item);
    }
    private void Searcher_FileExcludedEvent(object? sender, EventArgs e)
    {
        resultsControl1.IncrementExcludedCount();
    }
    private void Searcher_SearchCompleteEvent(object? sender, EventArgs e)
    {
        resultsControl1.SetSearchComplete();
        btnSearch.Text = "Search";
        btnSearch.Enabled = true;
    }

    private void btnResetTypes_Click(object sender, EventArgs e)
    {
        rtFileTypes.Clear();
        rtFileTypes.Lines =
        [
            ".txt"
            ,".cs"
            ,".vb"
            ,".sql"
            ,".csproj"
            ,".vbproj"
            ,".config"
            ,".DotSettings"
            ,".settings"
            ,".sqlproj"
            ,".sln"
            ,".js"
            ,".ts"
            ,".c"
            ,".cpp"
            ,".py"
            ,".htm"
            ,".html"
            ,".htmx"
            ,".cshtml"
            ,".ascx"
            ,".aspx"
            ,".ashx"
            ,".asmx"
            ,".asax"
            ,".php"
            ,".css"
            ,".sitemap"
            ,".ps1"
            ,".psd1"
            ,".psm1"
            ,".json"
            ,".xml"
            ,".csv"
            ,".yml"
            ,".yaml"
            ,".md"
            ,".rdl"
            ,".rdlc"
            ,".cmd"
            ,".bat"
        ];
    }



    private void chkFilterLineContains_CheckedChanged(object sender, EventArgs e)
    {
        if (chkFilterFileContains.Checked)
        {
            chkFilterFileContains.CheckedChanged -= chkFilterFileContains_CheckedChanged!;
            chkFilterFileContains.Checked = false;
            chkFilterFileContains.CheckedChanged += chkFilterFileContains_CheckedChanged!;
        }
    }
    private void chkFilterFileContains_CheckedChanged(object sender, EventArgs e)
    {
        if (chkFilterLineContains.Checked)
        {
            chkFilterLineContains.CheckedChanged -= chkFilterLineContains_CheckedChanged!;
            chkFilterLineContains.Checked = false;
            chkFilterLineContains.CheckedChanged += chkFilterLineContains_CheckedChanged!;
        }
    }
}
