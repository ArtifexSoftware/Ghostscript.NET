namespace Ghostscript.NET.Viewer
{
    partial class FMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNext = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPrev = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFirst = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLast = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblSystemInformation = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblGsVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.panTop = new System.Windows.Forms.Panel();
            this.panMenuAndToolbar = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tbPageFirst = new System.Windows.Forms.ToolStripButton();
            this.tbPagePrevious = new System.Windows.Forms.ToolStripButton();
            this.tbPageNumber = new System.Windows.Forms.ToolStripTextBox();
            this.tbTotalPages = new System.Windows.Forms.ToolStripLabel();
            this.tbPageNext = new System.Windows.Forms.ToolStripButton();
            this.tbPageLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tpZoomOut = new System.Windows.Forms.ToolStripButton();
            this.tpZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbDebug = new System.Windows.Forms.ToolStripButton();
            this.panGDN = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pbPage = new System.Windows.Forms.PictureBox();
            this.mnuMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panTop.SuspendLayout();
            this.panMenuAndToolbar.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panGDN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPage)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.BackColor = System.Drawing.Color.White;
            this.mnuMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuHelp});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(515, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen,
            this.mnuFileClose,
            this.toolStripMenuItem1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(103, 22);
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // mnuFileClose
            // 
            this.mnuFileClose.Name = "mnuFileClose";
            this.mnuFileClose.Size = new System.Drawing.Size(103, 22);
            this.mnuFileClose.Text = "&Close";
            this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(103, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNext,
            this.mnuPrev,
            this.mnuFirst,
            this.mnuLast});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "View";
            // 
            // mnuNext
            // 
            this.mnuNext.Name = "mnuNext";
            this.mnuNext.Size = new System.Drawing.Size(148, 22);
            this.mnuNext.Text = "Next Page";
            this.mnuNext.Click += new System.EventHandler(this.mnuNext_Click);
            // 
            // mnuPrev
            // 
            this.mnuPrev.Name = "mnuPrev";
            this.mnuPrev.Size = new System.Drawing.Size(148, 22);
            this.mnuPrev.Text = "Previous Page";
            this.mnuPrev.Click += new System.EventHandler(this.mnuPrev_Click);
            // 
            // mnuFirst
            // 
            this.mnuFirst.Name = "mnuFirst";
            this.mnuFirst.Size = new System.Drawing.Size(148, 22);
            this.mnuFirst.Text = "First Page";
            this.mnuFirst.Click += new System.EventHandler(this.mnuFirst_Click);
            // 
            // mnuLast
            // 
            this.mnuLast.Name = "mnuLast";
            this.mnuLast.Size = new System.Drawing.Size(148, 22);
            this.mnuLast.Text = "Last Page";
            this.mnuLast.Click += new System.EventHandler(this.mnuLast_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "Help";
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(107, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSystemInformation,
            this.lblGsVersion});
            this.statusStrip1.Location = new System.Drawing.Point(0, 580);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(788, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblSystemInformation
            // 
            this.lblSystemInformation.AutoSize = false;
            this.lblSystemInformation.Name = "lblSystemInformation";
            this.lblSystemInformation.Size = new System.Drawing.Size(240, 17);
            this.lblSystemInformation.Text = "System information";
            this.lblSystemInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblGsVersion
            // 
            this.lblGsVersion.Name = "lblGsVersion";
            this.lblGsVersion.Size = new System.Drawing.Size(117, 17);
            this.lblGsVersion.Text = "Ghostscript version...";
            // 
            // panTop
            // 
            this.panTop.Controls.Add(this.panMenuAndToolbar);
            this.panTop.Controls.Add(this.panGDN);
            this.panTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTop.Location = new System.Drawing.Point(0, 0);
            this.panTop.Name = "panTop";
            this.panTop.Size = new System.Drawing.Size(788, 56);
            this.panTop.TabIndex = 2;
            // 
            // panMenuAndToolbar
            // 
            this.panMenuAndToolbar.BackColor = System.Drawing.Color.White;
            this.panMenuAndToolbar.Controls.Add(this.toolStrip1);
            this.panMenuAndToolbar.Controls.Add(this.mnuMain);
            this.panMenuAndToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panMenuAndToolbar.Location = new System.Drawing.Point(0, 0);
            this.panMenuAndToolbar.Name = "panMenuAndToolbar";
            this.panMenuAndToolbar.Size = new System.Drawing.Size(515, 56);
            this.panMenuAndToolbar.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tbPageFirst,
            this.tbPagePrevious,
            this.tbPageNumber,
            this.tbTotalPages,
            this.tbPageNext,
            this.tbPageLast,
            this.toolStripLabel3,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.tpZoomOut,
            this.tpZoomIn,
            this.toolStripLabel4,
            this.toolStripSeparator2,
            this.tbDebug});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(515, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(51, 28);
            this.toolStripLabel1.Text = "    Page: ";
            // 
            // tbPageFirst
            // 
            this.tbPageFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPageFirst.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_first;
            this.tbPageFirst.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbPageFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPageFirst.Name = "tbPageFirst";
            this.tbPageFirst.Size = new System.Drawing.Size(28, 28);
            this.tbPageFirst.Text = "toolStripButton1";
            this.tbPageFirst.ToolTipText = "First Page";
            this.tbPageFirst.Click += new System.EventHandler(this.tbPageFirst_Click);
            // 
            // tbPagePrevious
            // 
            this.tbPagePrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPagePrevious.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_previous;
            this.tbPagePrevious.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbPagePrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPagePrevious.Name = "tbPagePrevious";
            this.tbPagePrevious.Size = new System.Drawing.Size(28, 28);
            this.tbPagePrevious.Text = "toolStripButton2";
            this.tbPagePrevious.ToolTipText = "Previous Page";
            this.tbPagePrevious.Click += new System.EventHandler(this.tbPagePrevious_Click);
            // 
            // tbPageNumber
            // 
            this.tbPageNumber.Name = "tbPageNumber";
            this.tbPageNumber.Size = new System.Drawing.Size(40, 31);
            this.tbPageNumber.TextChanged += new System.EventHandler(this.tbPageNumber_TextChanged);
            // 
            // tbTotalPages
            // 
            this.tbTotalPages.Name = "tbTotalPages";
            this.tbTotalPages.Size = new System.Drawing.Size(0, 28);
            // 
            // tbPageNext
            // 
            this.tbPageNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPageNext.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_next;
            this.tbPageNext.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbPageNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPageNext.Name = "tbPageNext";
            this.tbPageNext.Size = new System.Drawing.Size(28, 28);
            this.tbPageNext.Text = "toolStripButton3";
            this.tbPageNext.ToolTipText = "Next Page";
            this.tbPageNext.Click += new System.EventHandler(this.tbPageNext_Click);
            // 
            // tbPageLast
            // 
            this.tbPageLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPageLast.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_last;
            this.tbPageLast.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbPageLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPageLast.Name = "tbPageLast";
            this.tbPageLast.Size = new System.Drawing.Size(28, 28);
            this.tbPageLast.Text = "toolStripButton4";
            this.tbPageLast.ToolTipText = "LastPage";
            this.tbPageLast.Click += new System.EventHandler(this.tbPageLast_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(19, 28);
            this.toolStripLabel3.Text = "    ";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(19, 28);
            this.toolStripLabel2.Text = "    ";
            // 
            // tpZoomOut
            // 
            this.tpZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tpZoomOut.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_zoom_out;
            this.tpZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tpZoomOut.Name = "tpZoomOut";
            this.tpZoomOut.Size = new System.Drawing.Size(23, 28);
            this.tpZoomOut.Text = "-";
            this.tpZoomOut.ToolTipText = "Zoom Out";
            this.tpZoomOut.Click += new System.EventHandler(this.tpZoomOut_Click);
            // 
            // tpZoomIn
            // 
            this.tpZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tpZoomIn.Image = global::Ghostscript.NET.Viewer.Properties.Resources.tb_zoom_in;
            this.tpZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tpZoomIn.Name = "tpZoomIn";
            this.tpZoomIn.Size = new System.Drawing.Size(23, 28);
            this.tpZoomIn.Text = "+";
            this.tpZoomIn.ToolTipText = "Zoom In";
            this.tpZoomIn.Click += new System.EventHandler(this.tpZoomIn_Click);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(19, 28);
            this.toolStripLabel4.Text = "    ";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // tbDebug
            // 
            this.tbDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbDebug.Image = ((System.Drawing.Image)(resources.GetObject("tbDebug.Image")));
            this.tbDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDebug.Name = "tbDebug";
            this.tbDebug.Size = new System.Drawing.Size(46, 28);
            this.tbDebug.Text = "Debug";
            this.tbDebug.Click += new System.EventHandler(this.tbDebug_Click);
            // 
            // panGDN
            // 
            this.panGDN.BackColor = System.Drawing.Color.White;
            this.panGDN.Controls.Add(this.pictureBox1);
            this.panGDN.Dock = System.Windows.Forms.DockStyle.Right;
            this.panGDN.Location = new System.Drawing.Point(515, 0);
            this.panGDN.Name = "panGDN";
            this.panGDN.Size = new System.Drawing.Size(273, 56);
            this.panGDN.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImage = global::Ghostscript.NET.Viewer.Properties.Resources.ghostscript_dotnet;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(12, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(261, 55);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(788, 4);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 576);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(788, 4);
            this.panel2.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.Color.DarkGray;
            this.panel3.Controls.Add(this.pbPage);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(4);
            this.panel3.Size = new System.Drawing.Size(788, 516);
            this.panel3.TabIndex = 5;
            // 
            // pbPage
            // 
            this.pbPage.Location = new System.Drawing.Point(4, 4);
            this.pbPage.Name = "pbPage";
            this.pbPage.Size = new System.Drawing.Size(126, 118);
            this.pbPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbPage.TabIndex = 1;
            this.pbPage.TabStop = false;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 602);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panTop);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MinimumSize = new System.Drawing.Size(540, 420);
            this.Name = "FMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ghostscript.NET.Viewer";
            this.Load += new System.EventHandler(this.FMain_Load);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panTop.ResumeLayout(false);
            this.panMenuAndToolbar.ResumeLayout(false);
            this.panMenuAndToolbar.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panGDN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileClose;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuNext;
        private System.Windows.Forms.ToolStripMenuItem mnuPrev;
        private System.Windows.Forms.ToolStripMenuItem mnuFirst;
        private System.Windows.Forms.ToolStripMenuItem mnuLast;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.Panel panTop;
        private System.Windows.Forms.Panel panGDN;
        private System.Windows.Forms.Panel panMenuAndToolbar;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripButton tbPageFirst;
        private System.Windows.Forms.ToolStripButton tbPagePrevious;
        private System.Windows.Forms.ToolStripButton tbPageNext;
        private System.Windows.Forms.ToolStripButton tbPageLast;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox tbPageNumber;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStripStatusLabel lblGsVersion;
        private System.Windows.Forms.ToolStripLabel tbTotalPages;
        private System.Windows.Forms.PictureBox pbPage;
        private System.Windows.Forms.ToolStripStatusLabel lblSystemInformation;
        private System.Windows.Forms.ToolStripButton tpZoomOut;
        private System.Windows.Forms.ToolStripButton tpZoomIn;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbDebug;
    }
}

