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
            columnHeader7 = new ColumnHeader();
            cmsFiles = new ContextMenuStrip(components);
            panel1 = new Panel();
            rtDetails = new RichTextBox();
            statusStrip1 = new StatusStrip();
            tsProgress = new ToolStripProgressBar();
            tsStatus = new ToolStripStatusLabel();
            tsErrorsLink = new ToolStripStatusLabel();
            tsSep0 = new ToolStripStatusLabel();
            tsMatchesDesc = new ToolStripStatusLabel();
            tsMatches = new ToolStripStatusLabel();
            tsSep1 = new ToolStripStatusLabel();
            tsFilesScannedDesc = new ToolStripStatusLabel();
            tsFilesScanned = new ToolStripStatusLabel();
            tsSep2 = new ToolStripStatusLabel();
            tsExcludedDesc = new ToolStripStatusLabel();
            tsExcluded = new ToolStripStatusLabel();
            tsSep3 = new ToolStripStatusLabel();
            tsTotalDesc = new ToolStripStatusLabel();
            tsTotal = new ToolStripStatusLabel();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            splitContainer1.Size = new Size(1063, 708);
            splitContainer1.SplitterDistance = 341;
            splitContainer1.TabIndex = 0;
            // 
            // lvFiles
            // 
            lvFiles.BorderStyle = BorderStyle.FixedSingle;
            lvFiles.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader5, columnHeader2, columnHeader6, columnHeader3, columnHeader4, columnHeader7 });
            lvFiles.ContextMenuStrip = cmsFiles;
            lvFiles.Dock = DockStyle.Fill;
            lvFiles.Font = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lvFiles.FullRowSelect = true;
            lvFiles.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvFiles.Location = new Point(0, 0);
            lvFiles.MultiSelect = false;
            lvFiles.Name = "lvFiles";
            lvFiles.Size = new Size(1063, 341);
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
            columnHeader4.Text = "File Path";
            columnHeader4.Width = 200;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Full Path";
            // 
            // cmsFiles
            // 
            cmsFiles.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            cmsFiles.Name = "contextMenuStrip1";
            cmsFiles.ShowImageMargin = false;
            cmsFiles.Size = new Size(36, 4);
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
            panel1.Size = new Size(1063, 363);
            panel1.TabIndex = 0;
            // 
            // rtDetails
            // 
            rtDetails.BorderStyle = BorderStyle.None;
            rtDetails.Dock = DockStyle.Fill;
            rtDetails.Font = new Font("Courier New", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtDetails.Location = new Point(1, 1);
            rtDetails.Name = "rtDetails";
            rtDetails.Size = new Size(1061, 361);
            rtDetails.TabIndex = 0;
            rtDetails.Text = "";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsProgress, tsStatus, tsErrorsLink, tsSep0, tsMatchesDesc, tsMatches, tsSep1, tsFilesScannedDesc, tsFilesScanned, tsSep2, tsExcludedDesc, tsExcluded, tsSep3, tsTotalDesc, tsTotal });
            statusStrip1.Location = new Point(0, 711);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.ShowItemToolTips = true;
            statusStrip1.Size = new Size(1063, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsProgress
            // 
            tsProgress.MarqueeAnimationSpeed = 1;
            tsProgress.Name = "tsProgress";
            tsProgress.Size = new Size(200, 16);
            tsProgress.Step = 1;
            // 
            // tsStatus
            // 
            tsStatus.Name = "tsStatus";
            tsStatus.Size = new Size(352, 17);
            tsStatus.Spring = true;
            tsStatus.Text = "...";
            tsStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tsErrorsLink
            // 
            tsErrorsLink.IsLink = true;
            tsErrorsLink.LinkColor = Color.Blue;
            tsErrorsLink.Name = "tsErrorsLink";
            tsErrorsLink.Size = new Size(46, 17);
            tsErrorsLink.Text = "Errors...";
            tsErrorsLink.Visible = false;
            tsErrorsLink.VisitedLinkColor = Color.Blue;
            tsErrorsLink.Click += tsErrorsLink_Click;
            // 
            // tsSep0
            // 
            tsSep0.Name = "tsSep0";
            tsSep0.Size = new Size(10, 17);
            tsSep0.Text = "|";
            tsSep0.Visible = false;
            // 
            // tsMatchesDesc
            // 
            tsMatchesDesc.Name = "tsMatchesDesc";
            tsMatchesDesc.Size = new Size(87, 17);
            tsMatchesDesc.Text = "Matching Files:";
            tsMatchesDesc.ToolTipText = "Number of files containing a search term in their name or path.";
            // 
            // tsMatches
            // 
            tsMatches.Name = "tsMatches";
            tsMatches.Size = new Size(16, 17);
            tsMatches.Text = "...";
            // 
            // tsSep1
            // 
            tsSep1.Name = "tsSep1";
            tsSep1.Size = new Size(10, 17);
            tsSep1.Text = "|";
            // 
            // tsFilesScannedDesc
            // 
            tsFilesScannedDesc.Name = "tsFilesScannedDesc";
            tsFilesScannedDesc.Size = new Size(81, 17);
            tsFilesScannedDesc.Text = "Files Scanned:";
            tsFilesScannedDesc.ToolTipText = "Number of files actually read (Path and/or Contents).";
            // 
            // tsFilesScanned
            // 
            tsFilesScanned.Name = "tsFilesScanned";
            tsFilesScanned.Size = new Size(16, 17);
            tsFilesScanned.Text = "...";
            // 
            // tsSep2
            // 
            tsSep2.Name = "tsSep2";
            tsSep2.Size = new Size(10, 17);
            tsSep2.Text = "|";
            // 
            // tsExcludedDesc
            // 
            tsExcludedDesc.Name = "tsExcludedDesc";
            tsExcludedDesc.Size = new Size(84, 17);
            tsExcludedDesc.Text = "Excluded Files:";
            tsExcludedDesc.ToolTipText = "Number of files ignored due to file type or other selected filters.";
            // 
            // tsExcluded
            // 
            tsExcluded.Name = "tsExcluded";
            tsExcluded.Size = new Size(16, 17);
            tsExcluded.Text = "...";
            // 
            // tsSep3
            // 
            tsSep3.Name = "tsSep3";
            tsSep3.Size = new Size(10, 17);
            tsSep3.Text = "|";
            // 
            // tsTotalDesc
            // 
            tsTotalDesc.Name = "tsTotalDesc";
            tsTotalDesc.Size = new Size(61, 17);
            tsTotalDesc.Text = "Total Files:";
            tsTotalDesc.ToolTipText = "Total files identified in the specified Path.";
            // 
            // tsTotal
            // 
            tsTotal.Name = "tsTotal";
            tsTotal.Size = new Size(16, 17);
            tsTotal.Text = "...";
            // 
            // ResultsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(statusStrip1);
            Controls.Add(splitContainer1);
            Name = "ResultsControl";
            Size = new Size(1063, 733);
            SizeChanged += ResultsControl_SizeChanged;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private ColumnHeader columnHeader7;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar tsProgress;
        private ToolStripStatusLabel tsStatus;
        private ToolStripStatusLabel tsMatchesDesc;
        private ToolStripStatusLabel tsSep1;
        private ToolStripStatusLabel tsExcludedDesc;
        private ToolStripStatusLabel tsSep2;
        private ToolStripStatusLabel tsTotalDesc;
        private ToolStripStatusLabel tsMatches;
        private ToolStripStatusLabel tsExcluded;
        private ToolStripStatusLabel tsTotal;
        private ToolStripStatusLabel tsFilesScannedDesc;
        private ToolStripStatusLabel tsFilesScanned;
        private ToolStripStatusLabel tsSep3;
        private ToolTip toolTip1;
        private ToolStripStatusLabel tsErrorsLink;
        private ToolStripStatusLabel tsSep0;
    }
}
