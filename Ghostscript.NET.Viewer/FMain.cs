//
// FMain.cs
// This file is part of Ghostscript.NET.Viewer project
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Ghostscript.NET.Viewer;

namespace Ghostscript.NET.Viewer
{
    public partial class FMain : Form
    {
        private GhostscriptViewer _viewer;
        private GhostscriptVersionInfo _gsVersion = GhostscriptVersionInfo.GetLastInstalledVersion();
        private StringBuilder _stdOut = new StringBuilder();
        private StringBuilder _stdErr = new StringBuilder();
        private bool _supressPageNumberChangeEvent = false;

        public FMain()
        {
            InitializeComponent();

            this.Text = Program.NAME;

            pbPage.Width = 100;
            pbPage.Height = 100;

            _viewer = new GhostscriptViewer();
            _viewer.AttachStdIO(new GhostscriptStdIOHandler(_stdOut, _stdErr));

            _viewer.DisplaySize += new GhostscriptViewerViewEventHandler(_viewer_DisplaySize);
            _viewer.DisplayUpdate += new GhostscriptViewerViewEventHandler(_viewer_DisplayUpdate);
            _viewer.DisplayPage += new GhostscriptViewerViewEventHandler(_viewer_DisplayPage);
        }

        void _viewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            pbPage.Image = e.Image;
        }

        void _viewer_DisplayUpdate(object sender, GhostscriptViewerViewEventArgs e)
        {
            pbPage.Invalidate();
            pbPage.Update();
        }

        void _viewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            pbPage.Invalidate();
            pbPage.Update();

            _supressPageNumberChangeEvent = true;
            tbPageNumber.Text = _viewer.CurrentPageNumber.ToString();
            _supressPageNumberChangeEvent = false;

            tbTotalPages.Text = " / " + _viewer.LastPageNumber.ToString();
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            lblSystemInformation.Text = "Operating system: " + (Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit") + "   " +
                "Process: " + (Environment.Is64BitProcess ? "64-bit" : "32-bit");

            lblGsVersion.Text = "Rasterizer: " + _gsVersion.LicenseType.ToString() + " Ghostscript " + 
                _gsVersion.Version.ToString() + " (" + Path.GetFileName(_gsVersion.DllPath) + ")";
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open PDF file";
            ofd.Filter = "PDF, PS, EPS files|*.pdf;*.ps;*.eps";

            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                mnuFileClose_Click(this, null);

                _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> OPEN");

                _viewer.Open(ofd.FileName, _gsVersion, false);

                this.Text = System.IO.Path.GetFileName(ofd.FileName) + " - " + Program.NAME;
            }
        }

        private void mnuFileClose_Click(object sender, EventArgs e)
        {
            _viewer.Close();

            _stdOut.Clear();
            _stdErr.Clear();

            pbPage.Image = null;
            this.Text = Program.NAME;
            tbPageNumber.Text = string.Empty;
            tbTotalPages.Text = string.Empty;
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void tbPageFirst_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW FIRST PAGE");
            _viewer.ShowFirstPage();
        }

        private void tbPagePrevious_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW PREVIOUS PAGE");
            _viewer.ShowPreviousPage();
        }

        private void tbPageNext_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW NEXT PAGE");
            _viewer.ShowNextPage();
        }

        private void tbPageLast_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW LAST PAGE");
            _viewer.ShowLastPage();
        }

        private void tbPageNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_supressPageNumberChangeEvent)
                {
                    if (tbPageNumber.Text.Length > 0)
                    {
                        _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW PAGE " + tbPageNumber.Text);
                        _viewer.ShowPage(int.Parse(tbPageNumber.Text));
                    }
                }
            }
            catch { }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Powered by Ghostscript.NET & Josip Habjan (habjan@gmail.com)", "About " + Program.NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuNext_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW NEXT PAGE");
            _viewer.ShowNextPage();
        }

        private void mnuPrev_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW PREVIOUS PAGE");
            _viewer.ShowPreviousPage();
        }

        private void mnuFirst_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW LAST PAGE");
            _viewer.ShowFirstPage();
        }

        private void mnuLast_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> SHOW LAST PAGE");
            _viewer.ShowLastPage();
        }

        private void tpZoomIn_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> ZOOM IN");
            _viewer.ZoomIn();
        }

        private void tpZoomOut_Click(object sender, EventArgs e)
        {
            _stdOut.AppendLine("@GSNET_VIEWER -> COMMAND -> ZOOM OUT");
            _viewer.ZoomOut();
        }

        private void tbDebug_Click(object sender, EventArgs e)
        {

            FDebug debug = new FDebug();
            debug.txtDebug.AppendText("StdOut:\r\n");
            debug.txtDebug.AppendText("---------------------------------------------------------------------------\r\n");
            debug.txtDebug.AppendText(_stdOut.ToString() + "\r\n");
            debug.txtDebug.AppendText("===========================================================================\r\n");
            debug.txtDebug.AppendText("StdErr:\r\n");
            debug.txtDebug.AppendText("---------------------------------------------------------------------------\r\n");
            debug.txtDebug.AppendText(_stdErr.ToString() + "\r\n");
            debug.txtDebug.AppendText("===========================================================================\r\n");
            debug.ShowDialog(this);
            debug.Dispose();
        }
    }
}
