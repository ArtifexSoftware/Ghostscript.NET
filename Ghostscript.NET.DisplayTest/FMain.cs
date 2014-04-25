using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ghostscript.NET;
using Ghostscript.NET.Viewer;
using Ghostscript.NET.Interpreter;

namespace Ghostscript.NET.DisplayTest
{
    public partial class FMain : Form
    {
        private GhostscriptViewer _viewer;
        private FPreview _preview = new FPreview();
        private StdIOHandler _stdioHandler;

        public FMain()
        {
            InitializeComponent();

            _stdioHandler = new StdIOHandler(txtOutput);
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            txtOutput.AppendText("Is64BitOperatingSystem: " + System.Environment.Is64BitOperatingSystem.ToString() + "\r\n");
            txtOutput.AppendText("Is64BitProcess: " + System.Environment.Is64BitProcess.ToString() + "\r\n");

            _preview.Show();
            this.Show();

            GhostscriptVersionInfo gvi =  GhostscriptVersionInfo.GetLastInstalledVersion();

            _viewer = new GhostscriptViewer();

            _viewer.AttachStdIO(_stdioHandler);
            
            _viewer.DisplaySize += new GhostscriptViewerViewEventHandler(_viewer_DisplaySize);
            _viewer.DisplayUpdate += new GhostscriptViewerViewEventHandler(_viewer_DisplayUpdate);
            _viewer.DisplayPage += new GhostscriptViewerViewEventHandler(_viewer_DisplayPage);

            _viewer.Open(gvi, true);
        }

        void _viewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            _preview.pbDisplay.Invalidate();
            _preview.pbDisplay.Update();            
        }

        void _viewer_DisplayUpdate(object sender, GhostscriptViewerViewEventArgs e)
        {
            _preview.pbDisplay.Invalidate();
            _preview.pbDisplay.Update();
        }

        void _viewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            _preview.pbDisplay.Image = e.Image;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            _viewer.Interpreter.Run(txtPostscript.Text);
            _preview.Activate();
        }

        private void FMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _viewer.Dispose();
            _viewer = null;
        }
    }
}
