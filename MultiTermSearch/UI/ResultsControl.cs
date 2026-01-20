using System.Data;
using System.Diagnostics;
using System.Text;
using MultiTermSearch.Classes;
using MultiTermSearch.UI;

namespace MultiTermSearch;

public partial class ResultsControl : UserControl
{
    private List<FileResult> _filesMatching = new List<FileResult>();
    private List<FileResult> _filesWithErrors = new List<FileResult>();
    private string _rootDir = string.Empty;
    private readonly Color _defaultBackColor;
    private Color _highlightColor = Color.Khaki;
    private int _millisecondRefreshFrequency = 50;   // this limits the UI from refreshing too often and making it non-responsive when trying to Cancel or view results
    private DateTime _lastFileResultUpdate = DateTime.UtcNow;
    private DateTime _lastExcludedFileCountUpdate = DateTime.UtcNow;
    private DateTime _lastScannedFileCountUpdate = DateTime.UtcNow;
    private FileResult? _selectedFile = null;
    private string _selectedFilePath = string.Empty;
    private int _totalFilesCount = 0;
    private int _filesMatchingCount = 0;
    private int _filesExcludedCount = 0;
    private int _filesSearchedCount = 0;
    private int _filesWithErrorCount = 0;
    private bool _showAllLines = false;
    private Stopwatch _searchTimer = new Stopwatch();

    ToolStripButton cmsFiles_ButtonCopySelectedFileName = null!;
    ToolStripButton cmsFiles_ButtonCopySelectedFilePath = null!;
    ToolStripButton cmsFiles_ButtonCopyAllFileNames = null!;
    ToolStripButton cmsFiles_ButtonCopyAllFilePaths = null!;
    ToolStripSeparator cmsSeparator = null!;
    ToolStripButton cmsLines_ButtonOpenToLine = null!;
    ToolStripButton cmsLines_ButtonCopyLine = null!;
    ToolStripButton cmsLines_ButtonShowAllLines = null!;

    ErrorDisplay? _errDisplay = null;

    private const string nppPath = @"C:\Program Files\Notepad++\notepad++.exe";
    private bool nppExists => File.Exists(nppPath);
    private const int _defaultLineLimit = 100;


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

        _defaultBackColor = rtDetails.BackColor;

        SetupContextMenuStrip_Files();
        SetupContextMenuStrip_Lines();
    }


    public void SetupContextMenuStrip_Files()
    {
        cmsFiles_ButtonCopySelectedFileName = new ToolStripButton("Copy Selected File Name");
        cmsFiles_ButtonCopySelectedFilePath = new ToolStripButton("Copy Selected File Path");
        cmsFiles_ButtonCopyAllFileNames = new ToolStripButton("Copy All File Names");
        cmsFiles_ButtonCopyAllFilePaths = new ToolStripButton("Copy All File Paths");
        cmsSeparator = new ToolStripSeparator();

        cmsFiles_ButtonCopySelectedFileName.Click += CmsFilesButtonCopySelectedFileName_Click;
        cmsFiles_ButtonCopySelectedFilePath.Click += CmsFilesButtonCopySelectedFilePath_Click;
        cmsFiles_ButtonCopyAllFileNames.Click += CmsFilesButtonCopyAllFileNames_Click;
        cmsFiles_ButtonCopyAllFilePaths.Click += CmsFilesButtonCopyAllFilePaths_Click;
    }
    public void SetupContextMenuStrip_Lines()
    {
        cmsLines_ButtonOpenToLine = new ToolStripButton(nppExists ? "Open File To Line" : "Open File");
        cmsLines_ButtonCopyLine = new ToolStripButton("Copy Selected Line");
        cmsSeparator = new ToolStripSeparator();
        cmsLines_ButtonShowAllLines = new ToolStripButton("Show All Lines");
        cmsLines_ButtonShowAllLines.Enabled = false;

        cmsLines_ButtonOpenToLine.Click += CmsLinesButtonOpenToLine_Click;
        cmsLines_ButtonCopyLine.Click += CmsLinesButtonCopyLine_Click;
        cmsLines_ButtonShowAllLines.Click += CmsLinesButtonShowAllLines_Click;
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
        tsFilesScanned.Text = _filesSearchedCount.ToString();
        tsExcluded.Text = _filesExcludedCount.ToString();
        Results_UpdateFileList();
    }

    public void ClearResults()
    {
        _totalFilesCount = 0;
        _filesMatchingCount = 0;
        _filesExcludedCount = 0;
        _filesSearchedCount = 0;
        _filesWithErrorCount = 0;
        StatusBar_ResetFields();

        _filesMatching.Clear();
        _filesWithErrors.Clear();
        lvFiles.Items.Clear();
        rtDetails.Clear();
        lvFiles.ContextMenuStrip = null;

        _errDisplay?.Close();
        _errDisplay?.Dispose();
        _errDisplay = null;
    }
    private void StatusBar_ResetFields()
    {
        tsStatus.Text = "...";
        tsTotal.Text = "...";
        tsExcluded.Text = "...";
        tsFilesScanned.Text = "...";
        tsMatches.Text = "...";
        tsSep0.Visible = tsErrorsLink.Visible = false;
    }
    private void StatusBar_RefreshProgressBar()
    {
        // no files found...
        if (_totalFilesCount == 0)
        {
            tsProgress.Value = 0;
            return;
        }

        // no progress or finished... we want to show an empty bar for both
        if (_filesSearchedCount == 0 || _filesSearchedCount == _totalFilesCount)
        {
            tsProgress.Value = 0;
            return;
        }

        // we have some progress
        //   calcluate out the amount out of 100...
        int prog = Convert.ToInt32((Convert.ToDecimal(_filesSearchedCount + _filesExcludedCount) / Convert.ToDecimal(_totalFilesCount)) * 100.0m);

        // if the new progress is not a significant change over our current progress value... dont set it
        if (prog != tsProgress.Value)
        {
            // this backwards progress and then forwards tricks the control to skip its slow animation
            tsProgress.Value = Math.Min(100, prog + 2);
            tsProgress.Value = prog;
            statusStrip1.Update();
        }
    }

    public void HandleResult(FileResult? result)
    {
        // Always count the record to note that we finsihed looking at a file on the global status count...
        //    The more specific counts will be handled further below
        _filesSearchedCount++;


        // if the result was not null, then it is a match
        if (result is not null)
        {
            if (result.ErrorMessage is null)
            {
                _filesMatchingCount++;
                _filesMatching.Add(result);

                // We arnt expecting this value to be spammed that much... go ahead and always update it
                tsMatches.Text = _filesMatchingCount.ToString();

                // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
                if ((DateTime.UtcNow - _lastFileResultUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
                {
                    Results_UpdateFileList();
                    _lastFileResultUpdate = DateTime.UtcNow;
                }
            }
            else
            {
                _filesWithErrorCount++;
                _filesWithErrors.Add(result);
                tsErrorsLink.ToolTipText = $"{_filesWithErrorCount} files encountered an error while scanning.{Environment.NewLine}Click for details...";

                // If the link to view the errors is not yet visible... set it now
                if (!tsSep0.Visible)
                {
                    tsSep0.Visible = tsErrorsLink.Visible = true;
                }

                // If the user currently has the Error display up... lets be nice and udpate it automatically with any new errors as they come in.
                if (_errDisplay is not null)
                {
                    _errDisplay.UpdateErrors(_filesWithErrors);
                }
            }
        }

        // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
        if ((DateTime.UtcNow - _lastScannedFileCountUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
        {
            tsFilesScanned.Text = _filesSearchedCount.ToString();
            _lastScannedFileCountUpdate = DateTime.UtcNow;
        }

        // move the progress bar along
        StatusBar_RefreshProgressBar();
    }
    public void IncrementExcludedCount()
    {
        _filesExcludedCount++;

        // Limits the result control to only refresh once every {x} milliseconds to reduce flickering and freezing the UI
        if ((DateTime.UtcNow - _lastExcludedFileCountUpdate).TotalMilliseconds > _millisecondRefreshFrequency)
        {
            tsExcluded.Text = _filesExcludedCount.ToString();
            _lastExcludedFileCountUpdate = DateTime.UtcNow;
        }

        StatusBar_RefreshProgressBar();
    }
    public void SetTotalFileCount(int totalFiles)
    {
        _totalFilesCount = totalFiles;
        tsTotal.Text = _totalFilesCount.ToString();
    }

    private void OpenFile(string filePath, int openToLine = 1)
    {
        if (!File.Exists(filePath))
            return;

        // if we have a "better" editor installed go ahead and use it
        if (nppExists)
        {
            var proc = new Process();
            proc.StartInfo.FileName = nppPath;
            proc.StartInfo.Arguments = $"-n{openToLine} \"{filePath}\"";
            proc.Start();
        }
        else
        {
            Process.Start("notepad.exe", $"\"{filePath}\"");
        }
    }


    private void Results_UpdateFileList()
    {
        // dont let the user click anything while we are in the middle of redrawing its parent
        cmsFiles.Hide();
        cmsFiles.Enabled = false;
        cmsLines.Hide();
        cmsLines.Enabled = false;

        // order the results sort of based on the file system... helps keep results more coherent for now
        //   also save off the new index of the record that is currently selected
        _filesMatching = _filesMatching.OrderBy(r => r.FilePath).ToList();
        int selectedIndex = -1;
        if (!string.IsNullOrWhiteSpace(_selectedFilePath))
            selectedIndex = _filesMatching.FindIndex(r => r.FilePath == _selectedFilePath);


        // Rebuild the file result list from scratch
        //   dont draw it while we add items to reduce flashing and more quickly finish this piece
        //   also disable the index changed event so if the user is viewing a files details, we dont make them loose their place
        lvFiles.SelectedIndexChanged -= lvFiles_SelectedIndexChanged;
        this.SuspendLayout();
        lvFiles.BeginUpdate();
        lvFiles.Items.Clear();
        int fileCount = 1;
        foreach (var res in _filesMatching)
        {
            string[] rowValues = {
                fileCount.ToString()
                , res.FileName
                , string.Join(",", res.FoundTerms).Trim(',')
                , res.LineResults.Count.ToString()
                , res.MatchCount.ToString()
                , res.FilePath.Replace(_rootDir, "...\\")
                , res.FilePath
            };

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
        cmsLines.Enabled = true;
        rtDetails.ContextMenuStrip = cmsLines;
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

    private void Details_UpdateFileDetails()
    {
        rtDetails.Clear();

        if (_selectedFile is null)
        {
            rtDetails.ResumeLayout();
            rtDetails.PerformLayout();
            return;
        }
        if (_selectedFile.LineResults.Count <= _defaultLineLimit)
        {
            cmsLines_ButtonShowAllLines.Enabled = false;
        }
        else
        {
            cmsLines_ButtonShowAllLines.Enabled = !_showAllLines;
        }


        int maxLineNumLength = _selectedFile.LineResults.Last().LineNumber.ToString().Length;

        // loop through line by line adding the lines that had matches to the display
        rtDetails.SuspendLayout();
        var highlights = new Dictionary<int, int>();
        int lineStartIndex = 0;
        string lineHeader = string.Empty;
        string lineToAdd = string.Empty;
        int lineCount = 0;
        foreach (var line in _selectedFile.LineResults)
        {
            if (line.LineNumber == 0)
            {
                // if the first record is LineNumber 0, that means the file name matched and we are displaying it
                lineHeader = "File Name";
            }
            else
            {
                // this is a normal file content line, just prepend the current line number
                lineHeader = line.LineNumber.ToString().PadLeft(maxLineNumLength, ' ');
            }
            lineStartIndex += lineHeader.Length + 2;


            // Add the actual line to the control
            rtDetails.AppendText($"{lineHeader}: {line.Line}{Environment.NewLine}");

            // Loop through each match and select it, then apply the highlighting color to it
            foreach (var term in line.TermResults.Keys)
            {
                foreach (var index in line.TermResults[term])
                {
                    rtDetails.Select(lineStartIndex + index, term.Length);
                    rtDetails.SelectionBackColor = _highlightColor;
                }
            }

            //rtDetails.Select(rtDetails.Text.Length - 1, 1);
            //rtDetails.SelectionBackColor = _defaultBackColor;
            lineStartIndex = rtDetails.Text.Length;

            // we have displayed the current line and highlighted accordingly...
            //   now check if the user wants to display all additional lines or if they want to stop at the default limit
            lineCount++;
            if (lineCount >= _defaultLineLimit && !_showAllLines)
            {
                rtDetails.AppendText($"{Environment.NewLine}(Displaying first {_defaultLineLimit} lines...)");
                break;
            }
        }

        rtDetails.Select(rtDetails.Text.Length - 1, 1);
        rtDetails.SelectionBackColor = _defaultBackColor;
        rtDetails.ResumeLayout();
    }


    private void lvFiles_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lvFiles.SelectedIndices.Count == 0)
            return;

        // we should have a single record if we are here
        var item = lvFiles.SelectedItems[0];
        _selectedFilePath = item.SubItems[(int)ColIndexes.FullFilePath].Text;
        _selectedFile = _filesMatching.FirstOrDefault(f => f.FilePath == _selectedFilePath)!;
        _showAllLines = false;


        Details_UpdateFileDetails();
    }
    private void lvFiles_DoubleClick(object sender, EventArgs e)
    {
        OpenFile(_selectedFilePath);
    }



    private void cmsFiles_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = false; // set this here so the cms shows on the first click
        cmsFiles.Items.Clear();

        if (lvFiles.SelectedItems.Count > 0)
        {
            cmsFiles.Items.Add(cmsFiles_ButtonCopySelectedFileName);
            cmsFiles.Items.Add(cmsFiles_ButtonCopySelectedFilePath);
        }

        if (lvFiles.Items.Count > 0)
        {
            if (cmsFiles.Items.Count > 0)
                cmsFiles.Items.Add(cmsSeparator);
            cmsFiles.Items.Add(cmsFiles_ButtonCopyAllFileNames);
            cmsFiles.Items.Add(cmsFiles_ButtonCopyAllFilePaths);
        }
    }
    private void CmsFilesButtonCopyAllFilePaths_Click(object? sender, EventArgs e)
    {
        Clipboard.SetText(string.Join(Environment.NewLine, _filesMatching.Select(r => r.FilePath)));
    }
    private void CmsFilesButtonCopyAllFileNames_Click(object? sender, EventArgs e)
    {
        Clipboard.SetText(string.Join(Environment.NewLine, _filesMatching.Select(r => r.FileName)));
    }
    private void CmsFilesButtonCopySelectedFileName_Click(object? sender, EventArgs e)
    {
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FullFilePath].Text;
        var selectedFile = _filesMatching.FirstOrDefault(r => r.FilePath == selectedFilePath);
        if (selectedFile != null)
            Clipboard.SetText(selectedFile.FileName);
    }
    private void CmsFilesButtonCopySelectedFilePath_Click(object? sender, EventArgs e)
    {
        string selectedFilePath = lvFiles.SelectedItems[0].SubItems[(int)ColIndexes.FullFilePath].Text;
        var selectedFile = _filesMatching.FirstOrDefault(r => r.FilePath == selectedFilePath);
        if (selectedFile != null)
            Clipboard.SetText(selectedFile.FilePath);
    }



    private void cmsLines_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = false;  // set this here so the cms shows on the first click
        cmsLines.Items.Clear();

        if (_selectedFile is null)
            return;

        if (!_selectedFile.LineResults.Any())
            return;

        cmsLines.Items.Add(cmsLines_ButtonOpenToLine);
        cmsLines.Items.Add(cmsLines_ButtonCopyLine);
        cmsLines.Items.Add(cmsSeparator);
        cmsLines.Items.Add(cmsLines_ButtonShowAllLines);
    }
    private void CmsLinesButtonOpenToLine_Click(object? sender, EventArgs e)
    {
        if (_selectedFile is null)
            return;
        int charIndexOfCursor = rtDetails.GetCharIndexFromPosition(Cursor.Position);
        int lineOfCursor = rtDetails.GetLineFromCharIndex(charIndexOfCursor);
        int fileLineFromResultLine = _selectedFile.LineResults[lineOfCursor].LineNumber;
        OpenFile(_selectedFilePath, fileLineFromResultLine);

    }
    private void CmsLinesButtonCopyLine_Click(object? sender, EventArgs e)
    {
        if (_selectedFile is null)
            return;
        int charIndexOfCursor = rtDetails.GetCharIndexFromPosition(Cursor.Position);
        int lineOfCursor = rtDetails.GetLineFromCharIndex(charIndexOfCursor);
        Clipboard.SetText(_selectedFile.LineResults[lineOfCursor].Line);
    }
    private void CmsLinesButtonShowAllLines_Click(object? sender, EventArgs e)
    {
        _showAllLines = true;
        Details_UpdateFileDetails();
    }


    private void ResultsControl_SizeChanged(object sender, EventArgs e)
    {
        // make sure the results expand and contract along with the window
        AdjustFileResultColumnWidths();
    }

    private void tsErrorsLink_Click(object sender, EventArgs e)
    {
        // if the user is already viewing the error display... dont show another
        if (_errDisplay is not null)
            return;


        // Initialize a new Error Display and show it to the user
        _errDisplay = new ErrorDisplay(_rootDir, _filesWithErrors);
        _errDisplay.FormClosed += _errDisplay_FormClosed;
        _errDisplay.Show();
    }

    private void _errDisplay_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _errDisplay?.Dispose();
        _errDisplay = null;
    }

}
