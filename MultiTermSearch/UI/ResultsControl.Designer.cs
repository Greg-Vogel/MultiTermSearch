namespace MultiTermSearch
{
    partial class ResultsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            splitContainer1 = new SplitContainer();
            lvFiles = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            cmsFiles = new ContextMenuStrip(components);
            panel1 = new Panel();
            rtDetails = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvFiles);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Size = new Size(1063, 733);
            splitContainer1.SplitterDistance = 354;
            splitContainer1.TabIndex = 0;
            // 
            // lvFiles
            // 
            lvFiles.BorderStyle = BorderStyle.FixedSingle;
            lvFiles.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader5, columnHeader2, columnHeader6, columnHeader3, columnHeader4 });
            lvFiles.ContextMenuStrip = cmsFiles;
            lvFiles.Dock = DockStyle.Fill;
            lvFiles.Font = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvFiles.FullRowSelect = true;
            lvFiles.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvFiles.Location = new Point(0, 0);
            lvFiles.MultiSelect = false;
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(1063, 354);
            lvFiles.TabIndex = 0;
            lvFiles.UseCompatibleStateImageBehavior = false;
            lvFiles.View = View.Details;
            lvFiles.SelectedIndexChanged += lvFiles_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "#";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "File Name";
            columnHeader5.Width = 200;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Matching Terms";
            columnHeader2.Width = 150;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Matching Lines";
            columnHeader6.Width = 125;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Matches";
            columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Full Path";
            columnHeader4.Width = 200;
            // 
            // cmsFiles
            // 
            cmsFiles.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            cmsFiles.Name = "contextMenuStrip1";
            cmsFiles.ShowImageMargin = false;
            cmsFiles.Size = new Size(156, 26);
            cmsFiles.Opening += cmsFiles_Opening;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(rtDetails);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(1);
            panel1.Size = new Size(1063, 375);
            panel1.TabIndex = 0;
            // 
            // rtDetails
            // 
            rtDetails.BorderStyle = BorderStyle.None;
            rtDetails.Dock = DockStyle.Fill;
            rtDetails.Font = new Font("Courier New", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtDetails.Location = new Point(1, 1);
            rtDetails.Name = "rtDetails";
            rtDetails.Size = new Size(1061, 373);
            rtDetails.TabIndex = 0;
            rtDetails.Text = "";
            // 
            // ResultsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "ResultsControl";
            Size = new Size(1063, 733);
            SizeChanged += ResultsControl_SizeChanged;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel panel1;
        private RichTextBox rtDetails;
        private ListView lvFiles;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ContextMenuStrip cmsFiles;
    }
}
