using MultiTermSearch.Models;
using System.Data;

namespace MultiTermSearch;

public partial class ResultsControl : UserControl
{
    private List<FileResult> _results = new List<FileResult>();
    private Color _highlightColor = Color.Khaki;

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
    }

    public void ClearResults()
    {
        _results.Clear();
        lvFiles.Items.Clear();
        rtDetails.Clear();
    }

    public void AddResult(FileResult result)
    {
        // get the index we need to insert this new record at based on file path
        _results.Add(result);
        _results = _results.OrderBy(r => r.FilePath).ToList();


        // Rebuild the file result list from scratch
        //   dont draw it while we add items to reduce flashing and more quickly finish this piece
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


        // resize the columns to better fit the results we have so far
        AdjustFileResultColumnWidths();
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

    private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lvFiles.SelectedIndices.Count == 0)
            return;


        //rtDetails.AppendText(string.Join(',', lvFiles.SelectedIndices.Sel

        // we should have a single record if we are here
        var item = lvFiles.SelectedItems[0];
        var filePath = item.SubItems[(int)ColIndexes.FilePath].Text;
        DisplayDetails(filePath);
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

    private void ResultsControl_SizeChanged(object sender, EventArgs e)
    {
        // make sure the results expand and contract along with the window
        AdjustFileResultColumnWidths();
    }
}
