using MultiTermSearch.Logic;
using MultiTermSearch.Classes;
using MultiTermSearch.Events;
using MultiTermSearch.Properties;

namespace MultiTermSearch;

public partial class MTS : Form
{
    private MtsSettings _settings = null!;
    private SearchDriver _searcher = null!;
    private int _totalFiles = 0;
    private int _filesMatching = 0;
    private int _filesSearched = 0;

    public MTS()
    {
        InitializeComponent();
        DoubleBuffered = true;

        LoadSettings();
        SetFieldDefaults();
    }

    private void LoadSettings()
    {
        // Load last used settings and defaults from the user settings
        string settingString = Settings.Default.LastSettings;
        if (string.IsNullOrWhiteSpace(settingString))
        {
            _settings = new MtsSettings();
            return;
        }

        // deserialize setting string into its class for easier use
        try
        {
            var settings = System.Text.Json.JsonSerializer.Deserialize<MtsSettings>(settingString);
            _settings = settings ?? new MtsSettings();
            return;
        }
        catch
        {
            _settings = new MtsSettings();
            return;
        }
    }

    private void SetFieldDefaults()
    {
        // Apply those last used values to the fields
        txtPath.Text = _settings.Path;
        chkIncludeSubDir.Checked = _settings.IncludeSubDir;
        chkWholeWord.Checked = _settings.Options_MatchWholeWord;
        chkIgnoreCase.Checked = _settings.Options_IgnoreCase;
        chkExcludeLargeDir.Checked = _settings.Options_ExcludeLargeDirectories;
        chkFilterLineContains.Checked = _settings.Filters_LineContainsAll;
        chkFilterFileContains.Checked = _settings.Filters_FileContainsAll;
        rtFileTypes.Lines = _settings.IncludeTypes;
        switch (_settings.Target)
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
        cbThreads.SelectedItem = _settings.MaxThreadCount.ToString();
        this.ActiveControl = rtSearchTerms;
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
        if (_searcher != null && _searcher.SearchInProgress)
        {
            _searcher.CancelSearchAsync();
            UpdateStatusDisplay("Cancelling...");
            return;
        }

        StartSearch();
    }

    private void btnSetFileTypes_Click(object sender, EventArgs e)
    {
        rtFileTypes.Clear();
        rtFileTypes.Lines = _settings.SavedFileTypes;
    }

    private void StartSearch()
    {
        // Clear previous results
        resultsControl1.ClearResults();


        // Get the search inputs
        var inputs = GetInputs();

        // Save these inputs as the last used settings
        Task.Run(() => SaveSettings(inputs));

        // Start the actual search process
        if (_searcher is null)
        {
            _searcher = new SearchDriver();
            _searcher.FileListIdentifiedEvent += Searcher_FileListIdentifiedEvent;
            _searcher.FileSearchedEvent += Searcher_FileSearchedEvent;
            _searcher.SearchCompleteEvent += Searcher_SearchCompleteEvent;
        }
        _totalFiles = 0;
        _filesMatching = 0;
        _filesSearched = 0;
        _searcher.StartSearchAsync(inputs);
        resultsControl1.BeginResultUpdates(inputs.Path);

        // Update the status display for the users benefit
        UpdateStatusDisplay("Searching...");

        // Now that the search is actually running
        //   switch the 'search' button into a 'cancel' one
        btnSearch.Text = "Cancel";
        btnSearch.Enabled = true;
    }

    private void UpdateStatusDisplay(string? status)
    {
        tsStatus.Value = _totalFiles == 0 ? 0 : Convert.ToInt32(Convert.ToDecimal(_filesSearched) / Convert.ToDecimal(_totalFiles) * 100);
        tsMatches.Text = $"Files Matching: {_filesMatching}";
        tsTotal.Text = $"Files Scanned: {_filesSearched}";

        if (status == null)
            return;
        if (status == "Finished")
        {
            tsStatus.Value = 0;
            if (tsStatusLabel.Text == "Cancelling...")
            {
                tsStatusLabel.Text = $"Cancelled after {_searcher?.SearchTimer.Elapsed.TotalSeconds} seconds.";
                return;
            }
            else if (tsStatusLabel.Text == "Cancelled")
            {
                return;
            }
            tsStatusLabel.Text = $"Finished after {_searcher?.SearchTimer.Elapsed.TotalSeconds} seconds.";
        }
        else
        {
            tsStatusLabel.Text = status;
        }

    }

    private void Searcher_FileListIdentifiedEvent(object? sender, FileListIdentifiedEventArgs e)
    {
        _totalFiles = e.FileCount;
        UpdateStatusDisplay(null);
    }

    private void Searcher_FileSearchedEvent(object? sender, ItemAddedEventArgs e)
    {
        // increment the total files searched
        _filesSearched += 1;

        // if we actually got a match, count it and add it to the display
        if (e.Item is not null)
        {
            _filesMatching += 1;
            resultsControl1.AddResult(e.Item);
        }

        UpdateStatusDisplay(null);
    }
    private void Searcher_SearchCompleteEvent(object? sender, EventArgs e)
    {
        resultsControl1.EndResultUpdates();
        btnSearch.Text = "Search";
        btnSearch.Enabled = true;

        // Update some status text
        UpdateStatusDisplay("Finished");
    }

    private SearchInputs GetInputs()
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
            Target = GetInput_Target(),
            IncludeTypes = rtFileTypes.Lines,
            SearcherThreadCount = Convert.ToByte(cbThreads.SelectedItem),
        };
    }

    private SearchInputs.ESearchTargets GetInput_Target()
    {
        if (radTargetBoth.Checked)
            return SearchInputs.ESearchTargets.Both;
        else if (radTargetFileNames.Checked)
            return SearchInputs.ESearchTargets.FileNames;
        return SearchInputs.ESearchTargets.FileContents;
    }

    private void SaveSettings(SearchInputs inputs)
    {
        _settings.MergeInputs(inputs);
        Settings.Default.LastSettings = System.Text.Json.JsonSerializer.Serialize(_settings);
        Settings.Default.Save();
    }
}
