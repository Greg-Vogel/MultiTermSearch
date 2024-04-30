namespace MultiTermSearch.UI
{
    partial class ErrorDisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lvErrors = new ListView();
            colNumber = new ColumnHeader();
            colFileName = new ColumnHeader();
            colPath = new ColumnHeader();
            colError = new ColumnHeader();
            SuspendLayout();
            // 
            // lvErrors
            // 
            lvErrors.Columns.AddRange(new ColumnHeader[] { colNumber, colFileName, colPath, colError });
            lvErrors.Dock = DockStyle.Fill;
            lvErrors.Font = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvErrors.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvErrors.Location = new Point(0, 0);
            lvErrors.Name = "lvErrors";
            lvErrors.Size = new Size(1239, 640);
            lvErrors.TabIndex = 0;
            lvErrors.UseCompatibleStateImageBehavior = false;
            lvErrors.View = View.Details;
            // 
            // colNumber
            // 
            colNumber.Text = "#";
            // 
            // colFileName
            // 
            colFileName.Text = "File Name";
            // 
            // colPath
            // 
            colPath.Text = "File Path";
            // 
            // colError
            // 
            colError.Text = "Error";
            // 
            // ErrorDisplay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1239, 640);
            Controls.Add(lvErrors);
            MinimizeBox = false;
            Name = "ErrorDisplay";
            Text = "Error Scanning Files";
            ResumeLayout(false);
        }

        #endregion

        private ListView lvErrors;
        private ColumnHeader colNumber;
        private ColumnHeader colFileName;
        private ColumnHeader colPath;
        private ColumnHeader colError;
    }
}