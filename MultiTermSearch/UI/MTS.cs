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

    private void StartSearch()
    {
        // Clear previous results
        resultsControl1.ClearResults();

        // Get the search inputs
        var inputs = GetInputsFromUI();

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
        if (_searcher != null && _searcher.SearchInProgress)
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
}
