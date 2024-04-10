using System.Data;
using System.Diagnostics;
using MultiTermSearch.Classes;

namespace MultiTermSearch;

public partial class ResultsControl : UserControl
{
    private List<FileResult> _results = new List<FileResult>();
    private string _rootDir = string.Empty;
    private Color _highlightColor = Color.Khaki;
    private int _millisecondRefreshFrequency = 50;   // this limits the UI from refreshing too often and making it non-responsive when trying to Cancel or view results
    private DateTime _lastFileResultUpdate = DateTime.UtcNow;
    private DateTime _lastExcludedFileCountUpdate = DateTime.UtcNow;
    private DateTime _lastScannedFileCountUpdate = DateTime.UtcNow;
    private string _selectedFilePath = string.Empty;
    private int _totalFiles = 0;
    private int _filesMatching = 0;
    private int _filesExcluded = 0;
    private int _filesSearched = 0;
    private Stopwatch _searchTimer = new Stopwatch();

    ToolStripButton cmsButtonCopySelectedFileName = null!;
    ToolStripButton cmsButtonCopySelectedFilePath = null!;
    ToolStripButton cmsButtonCopyAllFileNames = null!;
    ToolStripButton cmsButtonCopyAllFilePaths = null!;
    ToolStripSeparator cmsSeparator = null!;

    private enum ColIndexes
    {
        FileNumber = 0,
        FileName = 1,
        MatchingTerms = 2,
        MatchingLines = 3,
        MatchCount = 4,
        ShortFilePath = 5,
        FullFilePath = 6,
    }

    public ResultsControl()
    {
        InitializeComponent();

        lvFiles.FullRowSelect = true;
        lvFiles.Columns[(int)ColIndexes.FullFilePath].Width = 0;
        lvFiles.GetType()?
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?
            .SetValue(lvFiles, true, null);

        SetupContextMenuStrip();
    }


    public void SetupContextMenuStrip()
    {
        cmsButtonCopySelectedFileName = new ToolStripButton("Copy Selected File Name");
        cmsButtonCopySelectedFilePath = new ToolStripButton("Copy Selected File Path");
        cmsButtonCopyAllFileNames = new ToolStripButton("Copy All File Names");
        cmsButtonCopyAllFilePaths = new ToolStripButton("Copy All File Paths");
        cmsSeparator = new ToolStripSeparator();

        cmsButtonCopySelectedFileName.Click += CmsButtonCopySelectedFileName_Click;
        cmsButtonCopySelectedFilePath.Click += CmsButtonCopySelectedFilePath_Click;
        cmsButtonCopyAllFileNames.Click += CmsButtonCopyAllFileNames_Click;
        cmsButtonCopyAllFilePaths.Click += CmsButtonCopyAllFilePaths_Click;
    }

    public void SetSearchBegin(string rootSearchDir)
    {
        _rootDir = rootSearchDir.EndsWith("\\") ? rootSearchDir : rootSearchDir + "\\";
        _searchTimer.Restart();
        tsStatus.Text = "Searching...";
    }
    public void SetSearchCancelling()
    {
        tsStatus.Text = "Cancelling...";
    }
    public void SetSearchComplete()
    {
        _searchTimer.Stop();

        tsProgress.Value = 0;
        if (tsStatus.Text == "Cancelling...")
            tsStatus.Text = $"Cancelled after {_searchTimer.Elapsed.TotalSeconds} seconds.";
        else
            tsStatus.Text = $"Finished after {_searchTimer.Elapsed.TotalSeconds} seconds.";

        // perform one last update to the UI to make sure all results are drawn
        //    since some could have came in during the refresh delay period
        tsFilesScanned.Text = _filesSearched.ToString();
        tsExcluded.Text = _filesExcluded.ToString();
        Results_UpdateFileList();
    }

    public void ClearResults()
    {
        _totalFiles = 0;
        _filesMatching = 0;
        _filesExcluded = 0;
        _filesSearched = 0;
        StatusBar_ResetFields();

        _results.Clear();
        lvFiles.Items.Clear();
        rtDetails.Clear();
        lvFiles.ContextMenuStrip = null;
    }
    private void StatusBar_ResetFields()
    {
        tsStatus.Text = "...";
        tsTotal.Text = "...";
        tsExcluded.Text = "...";
        tsFilesScanned.Text = "...";
        tsMatches.Text = "...";
    }
    private void StatusBar_RefreshProgressBar()
    {
        // no files found...
        if (_totalFiles == 0)
        {
            tsProgress.Value = 0;
            return;
        }

        // no progress or finished... we want to show an empty bar for both
        if (_filesSearched == 0 || _filesSearched == _totalFiles)
        {
            tsProgress.Value = 0;
            return;
        }

        // we have some progress
        //   calcluate out the amount out of 100...
        int prog = Convert.ToInt32((Convert.ToDecimal(_filesSearched + _filesExcluded) / Convert.ToDecimal(_totalFiles)) * 100.0m);

        // if the new progress is not a significant change over our current progress value... dont set it
        if (prog != tsProgress.Value)
        {
            // this backwards progress and then forwards tricks the control to skip its slow animation
            tsProgress.Value = Math.Min(100, prog + 2);
            tsProgress.Value = prog;
            statusStrip1.Update();
        }
    }

    public void AddResult(FileResult? result)
    {
        // if the result was not null, then it is a match
        if (result is not null)
        { 
            _filesMatching++;
            _results.Add(result);

            // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
            if ((DateTime.UtcNow - _lastFileResultUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
            {
                Results_UpdateFileList();
                _lastFileResultUpdate = DateTime.UtcNow;
            }

            // We arnt expecting this value to be spammed that much... go ahead and always update it
            tsMatches.Text = _filesMatching.ToString();
        }

        // always increment the search count even if the result is null
        //   we want to count the ones where no matches were found too
        _filesSearched++;

        // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
        if ((DateTime.UtcNow - _lastScannedFileCountUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
        {
            tsFilesScanned.Text = _filesSearched.ToString();
            _lastScannedFileCountUpdate = DateTime.UtcNow;
        }

        // move the progress bar along
        StatusBar_RefreshProgressBar();
    }
    public void IncrementExcludedCount()
    {
        _filesExcluded++;

        // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
        if ((DateTime.UtcNow - _lastExcludedFileCountUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
        {
            tsExcluded.Text = _filesExcluded.ToString();
            _lastExcludedFileCountUpdate = DateTime.UtcNow;
        }

        StatusBar_RefreshProgressBar();
    }
    public void SetTotalFileCount(int totalFiles)
    {
        _totalFiles = totalFiles;
        tsTotal.Text = _totalFiles.ToString();
    }



    private void Results_UpdateFileList()
    {
        // dont let the user click anything while we are in the middle of redrawing its parent
        cmsFiles.Hide();
        cmsFiles.Enabled = false;

        // order the results sort of based on the file system... helps keep results more coherent for now
        //   also save off the new index of the record that is currently selected
        _results = _results.OrderBy(r => r.FilePath).ToList();
        int selectedIndex = -1;
        if (!string.IsNullOrWhiteSpace(_selectedFilePath))
            selectedIndex = _results.FindIndex(r => r.FilePath == _selectedFilePath);


        // Rebuild the file result list from scratch
        //   dont draw it while we add items to reduce flashing and more quickly finish this piece
        //   also disable the index changed event so if the user is viewing a files details, we dont make them loose their place
        lvFiles.SelectedIndexChanged -= lvFiles_SelectedIndexChanged;
        this.SuspendLayout();
        lvFiles.BeginUpdate();
        lvFiles.Items.Clear();
        int fileCount = 1;
        foreach (var res in _results)
        {
            string[] rowValues = {fileCount.ToString()
            , res.FileName
            , string.Join(",", res.FoundTerms).Trim(',')
            , res.LineResults.Count.ToString()
            , res.MatchCount.ToString()
            , res.FilePath.Replace(_rootDir, "...\\")
            , res.FilePath };

            lvFiles.Items.Add(new ListViewItem(rowValues));

            fileCount++;
        }

        // resize the columns to better fit the results we have so far
        AdjustFileResultColumnWidths();

        // If the user was previously viewing a file, reselect it now that we might have changed its location and re-enable the index change event
        if (selectedIndex >= 0)
        {
            lvFiles.Items[selectedIndex].Focused = true;
            lvFiles.Items[selectedIndex].Selected = true;
            lvFiles.Items[selectedIndex].EnsureVisible();
        }
        lvFiles.SelectedIndexChanged += lvFiles_SelectedIndexChanged;


        // start drawing the UI again
        this.ResumeLayout();
        lvFiles.EndUpdate();

        // turn the context menu strip back on now that we are done updating its parent
        cmsFiles.Enabled = true;
        lvFiles.ContextMenuStrip = cmsFiles;
    }

    private void AdjustFileResultColumnWidths()
    {
        // resize all of the columns to fit their content... we dont want to cut any words short and force the user to manually expand a column
        lvFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        // hide any hidden columns
        lvFiles.Columns[(int)ColIndexes.FullFilePath].Width = 0;


        // Figure out if the column widths sumb up shorter than the whole lvFiles control itself
        //   If they do... expand the file path column to fill the space
        //   If they overflow past the width, just leave them alone
        int currentColWidth = 0;
        for (int i = 0; i < lvFiles.Columns.Count - 1; i++)
        {
            currentColWidth += lvFiles.Columns[i].Width;
        }
        int borderBuffer = 6;
        if (lvFiles.Width > currentColWidth + borderBuffer)
            lvFiles.Columns[(int)ColIndexes.ShortFilePath].Width += (lvFiles.Width - currentColWidth - borderBuffer);
    }


    private void Details_UpdateFileDetails(string filePath)
    {
        rtDetails.Clear();

        // lookup what item we are displaying details for by its unique filePath
        var fileResults = _results.FirstOrDefault(r => r.FilePath == filePath);
        if (fileResults is null)
            return;

        int maxLineNumLength = fileResults.LineResults.Last().LineNumber.ToString().Length;

        // loop through line by line adding the lines that had matches to the display
        //   Also record indexes and ranges to highlight later
        rtDetails.SuspendLayout();
        var highlights = new Dictionary<int, int>();
        int lineStartIndex = 0;
        string lineHeader = string.Empty;
        string lineToAdd = string.Empty;
        foreach (var line in fileResults.LineResults)
        {
            lineHeader = string.Format("{0}: ", line.LineNumber.ToString().PadLeft(maxLineNumLength, ' '));
            lineStartIndex += lineHeader.Length;

            lineToAdd = string.Format("{0}{1}{2}", lineHeader, line.Line, Environment.NewLine);

            rtDetails.AppendText(lineToAdd);

            foreach (var term in line.TermResults)
            {
                foreach (var index in term.IndexOfMatches)
                {
                    rtDetails.Select(lineStartIndex + index, term.Term.Length);
                    rtDetails.SelectionBackColor = _highlightColor;
                }
            }
            lineStartIndex = rtDetails.Text.Length;
        }
        rtDetails.ResumeLayout();
        rtDetails.PerformLayout();
    }
    public void Details_SetText(string text)
    {
        rtDetails.Clear();
        rtDetails.Text = text;
    }

    private void lvFiles_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lvFiles.SelectedIndices.Count == 0)
            return;

        // we should have a single record if we are here
        var item = lvFiles.SelectedItems[0];
        _selectedFilePath = item.SubItems[(int)ColIndexes.FullFilePath].Text;

        Details_UpdateFileDetails(_selectedFilePath);
    }


    private void CmsButtonCopyAllFilePaths_Click(object? sender, EventArgs e)
    {
        Clipboard.SetText(string.Join(Environment.NewLine, _results.Select(r => r.FilePath)));
    }
    private void CmsButtonCopyAllFileNames_Click(object? sender, EventArgs e)
    {
        Clipboard.SetText(string.Join(Environment.NewLine, _results.Select(r => r.FileName)));
    }

    private void CmsButtonCopySelectedFileName_Click(object? sender, EventArgs e)
    {
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FullFilePath].Text;
        var selectedFile = _results.FirstOrDefault(r => r.FilePath == selectedFilePath);
        if (selectedFile != null)
            Clipboard.SetText(selectedFile.FileName);
    }
    private void CmsButtonCopySelectedFilePath_Click(object? sender, EventArgs e)
    {
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FullFilePath].Text;
        var selectedFile = _results.FirstOrDefault(r => r.FilePath == selectedFilePath);
        if (selectedFile != null)
            Clipboard.SetText(selectedFile.FilePath);
    }
    private void CmsButtonCopySelectedFileResults_Click(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void cmsFiles_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        cmsFiles.Items.Clear();

        if (lvFiles.SelectedItems.Count > 0)
        {
            cmsFiles.Items.Add(cmsButtonCopySelectedFileName);
            cmsFiles.Items.Add(cmsButtonCopySelectedFilePath);
        }

        if (lvFiles.Items.Count > 0)
        {
            if (cmsFiles.Items.Count > 0)
                cmsFiles.Items.Add(cmsSeparator);
            cmsFiles.Items.Add(cmsButtonCopyAllFileNames);
            cmsFiles.Items.Add(cmsButtonCopyAllFilePaths);
        }
    }


    private void ResultsControl_SizeChanged(object sender, EventArgs e)
    {
        // make sure the results expand and contract along with the window
        AdjustFileResultColumnWidths();
    }
}
