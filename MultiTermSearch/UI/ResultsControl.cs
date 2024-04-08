using MultiTermSearch.Models;
using System.Data;

namespace MultiTermSearch;

public partial class ResultsControl : UserControl
{
    private List<FileResult> _results = new List<FileResult>();
    private Color _highlightColor = Color.Khaki;
    private int _lastResultCount = 0;
    private int _millisecondRefreshFrequency = 50;   // this limits the UI from refreshing too often and making it non-responsive when trying to Cancel or view results
    private System.Timers.Timer _updateTimer;

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
        FilePath = 5,
    }

    public ResultsControl()
    {
        InitializeComponent();

        lvFiles.FullRowSelect = true;
        _updateTimer = new System.Timers.Timer(_millisecondRefreshFrequency) { AutoReset = true };
        _updateTimer.Elapsed += UpdateTimer_ElapsedEvent;

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

    public void BeginResultUpdates()
    {
        _updateTimer.Start();
    }
    public void EndResultUpdates()
    {
        _updateTimer.Stop();

        // perform one last update to the UI to make sure all results are drawn
        UpdateFileList();
    }
    private void UpdateTimer_ElapsedEvent(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // if no items were found during this time... skip updating
        if (_lastResultCount == _results.Count)
            return;

        // We have something new to draw...
        //   Trigger the redraw back on the UI thread, not this timer thread
        lvFiles.Invoke(UpdateFileList);
    }


    public void ClearResults()
    {
        _results.Clear();
        lvFiles.Items.Clear();
        rtDetails.Clear();
        lvFiles.ContextMenuStrip = null;
        _lastResultCount = 0;
    }

    public void AddResult(FileResult result)
    {
        // always store the result sent to us
        _results.Add(result);
    }

    private void UpdateFileList()
    {
        // dont let the user click anything while we are in the middle of redrawing its parent
        cmsFiles.Hide();
        cmsFiles.Enabled = false;

        // order the results sort of based on the file system... helps keep results more coherent for now
        _results = _results.OrderBy(r => r.FilePath).ToList();


        // Rebuild the file result list from scratch
        //   dont draw it while we add items to reduce flashing and more quickly finish this piece
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
            , res.FilePath };

            lvFiles.Items.Add(new ListViewItem(rowValues));

            fileCount++;
        }
        lvFiles.EndUpdate();
        this.ResumeLayout();


        // resize the columns to better fit the results we have so far
        AdjustFileResultColumnWidths();

        // turn the context menu strip back on now that we are done updating its parent
        cmsFiles.Enabled = true;
        lvFiles.ContextMenuStrip = cmsFiles;

        // let our UI refresh checks know what we drew this time
        _lastResultCount = _results.Count;
    }

    private void AdjustFileResultColumnWidths()
    {
        lvFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        int currentColWidth = 0;
        for (int i = 0; i < lvFiles.Columns.Count - 1; i++)
        {
            currentColWidth += lvFiles.Columns[i].Width;
        }
        lvFiles.Columns[(int)ColIndexes.FilePath].Width = Math.Max(100, lvFiles.Width - currentColWidth - 4); // make the last column take up the rest of the space or at minimum 100px
    }

    private void DisplayDetails(string filePath)
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


    private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvFiles.SelectedIndices.Count == 0)
            return;

        // we should have a single record if we are here
        var item = lvFiles.SelectedItems[0];
        var filePath = item.SubItems[(int)ColIndexes.FilePath].Text;
        DisplayDetails(filePath);
    }


    private void ResultsControl_SizeChanged(object sender, EventArgs e)
    {
        // make sure the results expand and contract along with the window
        AdjustFileResultColumnWidths();
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
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FilePath].Text;
        var selectedFile = _results.FirstOrDefault(r => r.FilePath == selectedFilePath);
        if (selectedFile != null)
            Clipboard.SetText(selectedFile.FileName);
    }
    private void CmsButtonCopySelectedFilePath_Click(object? sender, EventArgs e)
    {
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FilePath].Text;
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
}
