namespace MultiTermSearch
{
    partial class MTS
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MTS));
            scMain = new SplitContainer();
            grpFilterPatterns = new GroupBox();
            chkFilterFileContains = new CheckBox();
            chkFilterLineContains = new CheckBox();
            grpOptions = new GroupBox();
            chkExcludeLargeDir = new CheckBox();
            chkIgnoreCase = new CheckBox();
            chkWholeWord = new CheckBox();
            label1 = new Label();
            panel3 = new Panel();
            rtFileTypes = new RichTextBox();
            grpSearchTarget = new GroupBox();
            radTargetContents = new RadioButton();
            radTargetFileNames = new RadioButton();
            radTargetBoth = new RadioButton();
            btnSearch = new Button();
            panel1 = new Panel();
            rtSearchTerms = new RichTextBox();
            btnBrowse = new Button();
            chkIncludeSubDir = new CheckBox();
            descSearchTerms = new Label();
            txtPath = new TextBox();
            descPath = new Label();
            resultsControl1 = new ResultsControl();
            statusStrip1 = new StatusStrip();
            tsStatus = new ToolStripProgressBar();
            tsStatusLabel = new ToolStripStatusLabel();
            tsSpacer = new ToolStripStatusLabel();
            tsMatches = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            tsTotal = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)scMain).BeginInit();
            scMain.Panel1.SuspendLayout();
            scMain.Panel2.SuspendLayout();
            scMain.SuspendLayout();
            grpFilterPatterns.SuspendLayout();
            grpOptions.SuspendLayout();
            panel3.SuspendLayout();
            grpSearchTarget.SuspendLayout();
            panel1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // scMain
            // 
            scMain.Dock = DockStyle.Fill;
            scMain.Location = new Point(0, 0);
            scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            scMain.Panel1.Controls.Add(grpFilterPatterns);
            scMain.Panel1.Controls.Add(grpOptions);
            scMain.Panel1.Controls.Add(label1);
            scMain.Panel1.Controls.Add(panel3);
            scMain.Panel1.Controls.Add(grpSearchTarget);
            scMain.Panel1.Controls.Add(btnSearch);
            scMain.Panel1.Controls.Add(panel1);
            scMain.Panel1.Controls.Add(btnBrowse);
            scMain.Panel1.Controls.Add(chkIncludeSubDir);
            scMain.Panel1.Controls.Add(descSearchTerms);
            scMain.Panel1.Controls.Add(txtPath);
            scMain.Panel1.Controls.Add(descPath);
            scMain.Panel1.Padding = new Padding(5, 0, 0, 0);
            scMain.Panel1MinSize = 236;
            // 
            // scMain.Panel2
            // 
            scMain.Panel2.Controls.Add(resultsControl1);
            scMain.Panel2.Controls.Add(statusStrip1);
            scMain.Panel2.Padding = new Padding(5, 5, 5, 0);
            scMain.Size = new Size(1415, 911);
            scMain.SplitterDistance = 295;
            scMain.TabIndex = 0;
            scMain.TabStop = false;
            // 
            // grpFilterPatterns
            // 
            grpFilterPatterns.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFilterPatterns.Controls.Add(chkFilterFileContains);
            grpFilterPatterns.Controls.Add(chkFilterLineContains);
            grpFilterPatterns.FlatStyle = FlatStyle.Flat;
            grpFilterPatterns.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpFilterPatterns.Location = new Point(11, 425);
            grpFilterPatterns.Margin = new Padding(5, 20, 3, 0);
            grpFilterPatterns.Name = "grpFilterPatterns";
            grpFilterPatterns.Size = new Size(281, 73);
            grpFilterPatterns.TabIndex = 24;
            grpFilterPatterns.TabStop = false;
            grpFilterPatterns.Text = "Filter Patterns";
            // 
            // chkFilterFileContains
            // 
            chkFilterFileContains.AutoSize = true;
            chkFilterFileContains.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkFilterFileContains.Location = new Point(17, 47);
            chkFilterFileContains.Name = "chkFilterFileContains";
            chkFilterFileContains.Size = new Size(170, 19);
            chkFilterFileContains.TabIndex = 2;
            chkFilterFileContains.Text = "File Must Contain All Terms";
            chkFilterFileContains.UseVisualStyleBackColor = true;
            // 
            // chkFilterLineContains
            // 
            chkFilterLineContains.AutoSize = true;
            chkFilterLineContains.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkFilterLineContains.Location = new Point(17, 22);
            chkFilterLineContains.Name = "chkFilterLineContains";
            chkFilterLineContains.Size = new Size(174, 19);
            chkFilterLineContains.TabIndex = 1;
            chkFilterLineContains.Text = "Line Must Contain All Terms";
            chkFilterLineContains.UseVisualStyleBackColor = true;
            // 
            // grpOptions
            // 
            grpOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpOptions.Controls.Add(chkExcludeLargeDir);
            grpOptions.Controls.Add(chkIgnoreCase);
            grpOptions.Controls.Add(chkWholeWord);
            grpOptions.FlatStyle = FlatStyle.Flat;
            grpOptions.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpOptions.Location = new Point(10, 312);
            grpOptions.Margin = new Padding(5, 20, 3, 0);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(281, 105);
            grpOptions.TabIndex = 23;
            grpOptions.TabStop = false;
            grpOptions.Text = "Search Options";
            // 
            // chkExcludeLargeDir
            // 
            chkExcludeLargeDir.AutoSize = true;
            chkExcludeLargeDir.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkExcludeLargeDir.Location = new Point(18, 72);
            chkExcludeLargeDir.Name = "chkExcludeLargeDir";
            chkExcludeLargeDir.Size = new Size(195, 19);
            chkExcludeLargeDir.TabIndex = 2;
            chkExcludeLargeDir.Text = "Exclude Dir: .git | node-modules";
            chkExcludeLargeDir.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreCase
            // 
            chkIgnoreCase.AutoSize = true;
            chkIgnoreCase.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkIgnoreCase.Location = new Point(18, 22);
            chkIgnoreCase.Name = "chkIgnoreCase";
            chkIgnoreCase.Size = new Size(88, 19);
            chkIgnoreCase.TabIndex = 0;
            chkIgnoreCase.Text = "Ignore Case";
            chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // chkWholeWord
            // 
            chkWholeWord.AutoSize = true;
            chkWholeWord.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkWholeWord.Location = new Point(18, 47);
            chkWholeWord.Name = "chkWholeWord";
            chkWholeWord.Size = new Size(129, 19);
            chkWholeWord.TabIndex = 1;
            chkWholeWord.Text = "Match Whole Word";
            chkWholeWord.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(10, 616);
            label1.Margin = new Padding(5, 20, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(104, 15);
            label1.TabIndex = 12;
            label1.Text = "Include File Types:";
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.BackColor = Color.Black;
            panel3.Controls.Add(rtFileTypes);
            panel3.Location = new Point(10, 636);
            panel3.Margin = new Padding(5, 5, 3, 0);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(1);
            panel3.Size = new Size(281, 253);
            panel3.TabIndex = 50;
            // 
            // rtFileTypes
            // 
            rtFileTypes.BorderStyle = BorderStyle.None;
            rtFileTypes.Dock = DockStyle.Fill;
            rtFileTypes.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtFileTypes.Location = new Point(1, 1);
            rtFileTypes.Margin = new Padding(1);
            rtFileTypes.Name = "rtFileTypes";
            rtFileTypes.Size = new Size(279, 251);
            rtFileTypes.TabIndex = 50;
            rtFileTypes.Text = ".*";
            // 
            // grpSearchTarget
            // 
            grpSearchTarget.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpSearchTarget.Controls.Add(radTargetContents);
            grpSearchTarget.Controls.Add(radTargetFileNames);
            grpSearchTarget.Controls.Add(radTargetBoth);
            grpSearchTarget.FlatStyle = FlatStyle.Flat;
            grpSearchTarget.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            grpSearchTarget.Location = new Point(10, 507);
            grpSearchTarget.Margin = new Padding(5, 20, 3, 0);
            grpSearchTarget.Name = "grpSearchTarget";
            grpSearchTarget.Size = new Size(281, 98);
            grpSearchTarget.TabIndex = 20;
            grpSearchTarget.TabStop = false;
            grpSearchTarget.Text = "What To Search";
            // 
            // radTargetContents
            // 
            radTargetContents.AutoSize = true;
            radTargetContents.Font = new Font("Segoe UI", 9F);
            radTargetContents.Location = new Point(18, 72);
            radTargetContents.Name = "radTargetContents";
            radTargetContents.Size = new Size(195, 19);
            radTargetContents.TabIndex = 22;
            radTargetContents.Text = "File Contents (excludes binaries)";
            radTargetContents.UseVisualStyleBackColor = true;
            // 
            // radTargetFileNames
            // 
            radTargetFileNames.AutoSize = true;
            radTargetFileNames.Font = new Font("Segoe UI", 9F);
            radTargetFileNames.Location = new Point(18, 47);
            radTargetFileNames.Name = "radTargetFileNames";
            radTargetFileNames.Size = new Size(83, 19);
            radTargetFileNames.TabIndex = 21;
            radTargetFileNames.Text = "File Names";
            radTargetFileNames.UseVisualStyleBackColor = true;
            // 
            // radTargetBoth
            // 
            radTargetBoth.AutoSize = true;
            radTargetBoth.Checked = true;
            radTargetBoth.Font = new Font("Segoe UI", 9F);
            radTargetBoth.Location = new Point(18, 22);
            radTargetBoth.Name = "radTargetBoth";
            radTargetBoth.Size = new Size(50, 19);
            radTargetBoth.TabIndex = 20;
            radTargetBoth.TabStop = true;
            radTargetBoth.Text = "Both";
            radTargetBoth.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSearch.Location = new Point(217, 283);
            btnSearch.Margin = new Padding(5, 5, 3, 0);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 11;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(rtSearchTerms);
            panel1.Location = new Point(10, 119);
            panel1.Margin = new Padding(5, 5, 3, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(1);
            panel1.Size = new Size(282, 159);
            panel1.TabIndex = 10;
            // 
            // rtSearchTerms
            // 
            rtSearchTerms.BorderStyle = BorderStyle.None;
            rtSearchTerms.Dock = DockStyle.Fill;
            rtSearchTerms.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtSearchTerms.Location = new Point(1, 1);
            rtSearchTerms.Margin = new Padding(1);
            rtSearchTerms.Name = "rtSearchTerms";
            rtSearchTerms.Size = new Size(280, 157);
            rtSearchTerms.TabIndex = 10;
            rtSearchTerms.Text = "";
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(217, 57);
            btnBrowse.Margin = new Padding(5, 5, 3, 0);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // chkIncludeSubDir
            // 
            chkIncludeSubDir.AutoSize = true;
            chkIncludeSubDir.Location = new Point(10, 60);
            chkIncludeSubDir.Margin = new Padding(5, 5, 5, 0);
            chkIncludeSubDir.Name = "chkIncludeSubDir";
            chkIncludeSubDir.Size = new Size(143, 19);
            chkIncludeSubDir.TabIndex = 2;
            chkIncludeSubDir.Text = "Include Subdirectories";
            chkIncludeSubDir.UseVisualStyleBackColor = true;
            // 
            // descSearchTerms
            // 
            descSearchTerms.AutoSize = true;
            descSearchTerms.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            descSearchTerms.Location = new Point(10, 99);
            descSearchTerms.Margin = new Padding(5, 20, 5, 0);
            descSearchTerms.Name = "descSearchTerms";
            descSearchTerms.Size = new Size(81, 15);
            descSearchTerms.TabIndex = 2;
            descSearchTerms.Text = "Search Terms:";
            // 
            // txtPath
            // 
            txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPath.BorderStyle = BorderStyle.FixedSingle;
            txtPath.Location = new Point(10, 29);
            txtPath.Margin = new Padding(5, 5, 3, 0);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(282, 23);
            txtPath.TabIndex = 1;
            // 
            // descPath
            // 
            descPath.AutoSize = true;
            descPath.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            descPath.Location = new Point(10, 9);
            descPath.Margin = new Padding(5, 0, 5, 0);
            descPath.Name = "descPath";
            descPath.Size = new Size(34, 15);
            descPath.TabIndex = 0;
            descPath.Text = "Path:";
            // 
            // resultsControl1
            // 
            resultsControl1.Dock = DockStyle.Fill;
            resultsControl1.Location = new Point(5, 5);
            resultsControl1.Name = "resultsControl1";
            resultsControl1.Size = new Size(1106, 884);
            resultsControl1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsStatus, tsStatusLabel, tsSpacer, tsMatches, toolStripStatusLabel3, tsTotal });
            statusStrip1.Location = new Point(5, 889);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1106, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            tsStatus.MarqueeAnimationSpeed = 1;
            tsStatus.Name = "tsStatus";
            tsStatus.Size = new Size(200, 16);
            tsStatus.Step = 1;
            tsStatus.Style = ProgressBarStyle.Continuous;
            // 
            // tsStatusLabel
            // 
            tsStatusLabel.Name = "tsStatusLabel";
            tsStatusLabel.Size = new Size(16, 17);
            tsStatusLabel.Text = "...";
            // 
            // tsSpacer
            // 
            tsSpacer.Name = "tsSpacer";
            tsSpacer.Size = new Size(637, 17);
            tsSpacer.Spring = true;
            // 
            // tsMatches
            // 
            tsMatches.Name = "tsMatches";
            tsMatches.Size = new Size(99, 17);
            tsMatches.Text = "Matching Files: ...";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(10, 17);
            toolStripStatusLabel3.Text = "|";
            // 
            // tsTotal
            // 
            tsTotal.Name = "tsTotal";
            tsTotal.Size = new Size(96, 17);
            tsTotal.Text = "Files Searched: ...";
            // 
            // MTS
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1415, 911);
            Controls.Add(scMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 950);
            Name = "MTS";
            Text = "MultiTerm Search";
            scMain.Panel1.ResumeLayout(false);
            scMain.Panel1.PerformLayout();
            scMain.Panel2.ResumeLayout(false);
            scMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)scMain).EndInit();
            scMain.ResumeLayout(false);
            grpFilterPatterns.ResumeLayout(false);
            grpFilterPatterns.PerformLayout();
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            panel3.ResumeLayout(false);
            grpSearchTarget.ResumeLayout(false);
            grpSearchTarget.PerformLayout();
            panel1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer scMain;
        private CheckBox chkIncludeSubDir;
        private Label descSearchTerms;
        private TextBox txtPath;
        private Label descPath;
        private StatusStrip statusStrip1;
        private Button btnBrowse;
        private RichTextBox rtSearchTerms;
        private Panel panel1;
        private GroupBox grpSearchTarget;
        private RadioButton radTargetContents;
        private RadioButton radTargetFileNames;
        private RadioButton radTargetBoth;
        private Button btnSearch;
        private Label label1;
        private Panel panel3;
        private RichTextBox rtFileTypes;
        private ResultsControl resultsControl1;
        private GroupBox grpOptions;
        private CheckBox chkIgnoreCase;
        private CheckBox chkWholeWord;
        private GroupBox grpFilterPatterns;
        private CheckBox chkFilterFileContains;
        private CheckBox chkFilterLineContains;
        private CheckBox chkExcludeLargeDir;
        private ToolStripProgressBar tsStatus;
        private ToolStripStatusLabel tsSpacer;
        private ToolStripStatusLabel tsMatches;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel tsTotal;
        private ToolStripStatusLabel tsStatusLabel;
    }
}