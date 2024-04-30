using MultiTermSearch.Classes;

namespace MultiTermSearch.UI
{
    public partial class ErrorDisplay : Form
    {
        private string? _rootDir;
        
        private enum ColIndexes
        {
            FileNumber = 0,
            FileName = 1,
            ShortFilePath = 2,
            Error = 3
        }
        
        public ErrorDisplay(string? rootDir, List<FileResult> errors) 
        {
            InitializeComponent();
            DoubleBuffered = true;

            _rootDir = rootDir;
            UpdateErrors(errors);
        }

        public void UpdateErrors(List<FileResult> errors)
        {
            this.SuspendLayout();
            lvErrors.BeginUpdate();
            lvErrors.Items.Clear();

            int fileCount = 1;
            foreach (var err in errors)
            {
                string[] rowValues = {
                    fileCount.ToString()
                    , err.FileName
                    , _rootDir is null ? err.FilePath : err.FilePath.Replace(_rootDir, "...\\")
                    , err.Error!
                };

                lvErrors.Items.Add(new ListViewItem(rowValues));

            }

            // resize all of the columns to fit their content... we dont want to cut any words short and force the user to manually expand a column
            lvErrors.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // the path isnt as important as the Error and it can take up way too much line length... hide it unless the user wants to expand it.
            lvErrors.Columns[(int)ColIndexes.ShortFilePath].Width = 250;

            // Figure out if the column widths sumb up shorter than the whole lvErrors control itself
            //   If they do... expand the Error column to fill the space
            //   If they overflow past the width, just leave them alone
            int currentColWidth = 0;
            for (int i = 0; i < lvErrors.Columns.Count - 1; i++)
            {
                currentColWidth += lvErrors.Columns[i].Width;
            }
            int borderBuffer = 6;
            if (lvErrors.Width > currentColWidth + borderBuffer)
                lvErrors.Columns[(int)ColIndexes.Error].Width += (lvErrors.Width - currentColWidth - borderBuffer);


            this.ResumeLayout();
            lvErrors.EndUpdate();
        }
    }
}
