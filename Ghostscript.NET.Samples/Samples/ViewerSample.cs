//
// ViewerSample.cs
// This file is part of Ghostscript.NET.Samples project
//
// Author: Artifex Software Inc. 
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
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
using System.IO;
using System.Threading;
using SkiaSharp;

// required Ghostscript.NET namespaces
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Viewer;

namespace Ghostscript.NET.Samples
{
    public class ViewerSample : ISample
    {
        private GhostscriptVersionInfo _lastInstalledVersion = null;
        private GhostscriptViewer _viewer = null;
        private SKBitmap _pdfPage = null;
        private ManualResetEventSlim _firstPageDone;
        private readonly object _firstPageLock = new object();
        private bool _handledFirstPage;

        public void Start()
        {
            // there can be multiple Ghostscript versions installed on the system
            // and we can choose which one we will use. In this sample we will use
            // the last installed Ghostscript version. We can choose if we want to 
            // use GPL or AFPL (commercial) version of the Ghostscript. By setting 
            // the parameters below we told that we want to fetch the last version 
            // of the GPL or AFPL Ghostscript and if both are available we prefer 
            // to use GPL version.

            _lastInstalledVersion =
                GhostscriptVersionInfo.GetLastInstalledVersion();

            _firstPageDone = new ManualResetEventSlim(false);
            bool pageReady = false;
            try
            {
                string inputPath = PrepareViewerInput();

                // create a new instance of the viewer
                _viewer = new GhostscriptViewer();

                // set the display update interval to 10 times per second. This value
                // is milliseconds based and updating display every 100 milliseconds
                // is optimal value. The smaller value you set the rasterizing will 
                // take longer as DisplayUpdate event will be raised more often.
                _viewer.ProgressiveUpdateInterval = 100;

                // attach three main viewer events
                _viewer.DisplaySize += new GhostscriptViewerViewEventHandler(_viewer_DisplaySize);
                _viewer.DisplayUpdate += new GhostscriptViewerViewEventHandler(_viewer_DisplayUpdate);
                _viewer.DisplayPage += new GhostscriptViewerViewEventHandler(_viewer_DisplayPage);

                // Open a PDF. Opening PipedOutputSample.ps directly with GhostscriptViewer
                // can block forever inside gsapi_run_string (DisplaySize fires, DisplayPage does not).
                Console.WriteLine("Rendering first page to Output\\ViewerSample.png ...");

                // Open on a background thread so a stuck gsapi call cannot freeze the sample runner.
                Exception openError = null;
                Thread openThread = new Thread(() =>
                {
                    try
                    {
                        _viewer.Open(inputPath, _lastInstalledVersion, false);
                    }
                    catch (Exception ex)
                    {
                        openError = ex;
                        try { _firstPageDone?.Set(); } catch { }
                    }
                });
                openThread.IsBackground = true;
                openThread.Name = "GhostscriptViewer.Open";
                openThread.Start();

                pageReady = _firstPageDone.Wait(TimeSpan.FromSeconds(30));
                if (!pageReady)
                {
                    Console.WriteLine("Timed out waiting for the first page (check Ghostscript and the input file).");
                }

                if (openError != null)
                {
                    throw openError;
                }
            }
            finally
            {
                if (pageReady && _viewer != null)
                {
                    try
                    {
                        _viewer.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Viewer dispose: " + ex.Message);
                    }

                    _viewer = null;
                }
                else
                {
                    _viewer = null;
                }

                if (_firstPageDone != null)
                {
                    _firstPageDone.Dispose();
                    _firstPageDone = null;
                }
            }
        }

        // this is the first raised event before PDF page starts rasterizing. 
        // this event is raised only once per page showing and it tells us 
        // the dimensions of the PDF page and gives us page image reference.
        void _viewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            // store PDF page image reference
            _pdfPage = e.Image;
            Console.WriteLine($"DisplaySize: {e.MediaBox.Width}x{e.MediaBox.Height} (logical units).");
        }

        // this event is raised when a tile or the part of the page is rasterized
        // code in this event must be fast or it will slow down the Ghostscript
        // rasterizing. 
        void _viewer_DisplayUpdate(object sender, GhostscriptViewerViewEventArgs e)
        {
            // if we are displaying the image in the PictureBox we can update 
            // it by calling PictureBox.Invalidate() and PictureBox.Update()
            // methods. We dont need to set image reference again because
            // Ghostscript.NET is changing Image object directly in the
            // memory and does not create new SKBitmap instance.
        }

        // this is the last raised event after complete page is rasterized
        void _viewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            lock (_firstPageLock)
            {
                if (_handledFirstPage)
                {
                    return;
                }

                _handledFirstPage = true;
            }

            try
            {
                SKBitmap bmp = e.Image ?? _pdfPage;
                if (bmp == null)
                {
                    Console.WriteLine("DisplayPage: no bitmap to save.");
                    return;
                }

                Directory.CreateDirectory(SampleFiles.OutputDirectory);
                string outPath = Path.Combine(SampleFiles.OutputDirectory, "ViewerSample.png");
                using (SKImage image = SKImage.FromBitmap(bmp))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (Stream stream = File.Create(outPath))
                {
                    data.SaveTo(stream);
                }

                Console.WriteLine($"First page saved to {Path.GetFullPath(outPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save viewer output: " + ex.Message);
            }
            finally
            {
                _firstPageDone?.Set();
            }
        }

        private static string PrepareViewerInput()
        {
            string pdfPath;
            if (SampleFiles.TryGet("ViewerSample.pdf", out pdfPath))
            {
                return pdfPath;
            }

            if (SampleFiles.TryGet("ProcessorSample1.pdf", out pdfPath))
            {
                return pdfPath;
            }

            string psPath;
            if (!SampleFiles.TryGet("PipedOutputSample.ps", out psPath))
            {
                throw new FileNotFoundException("No viewer sample input file was found.");
            }

            string outputPdf = Path.Combine(SampleFiles.OutputDirectory, "ViewerSample-input.pdf");
            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                processor.Process(new[]
                {
                    "-dBATCH",
                    "-dNOPAUSE",
                    "-dNOPROMPT",
                    "-dNOSAFER",
                    "-sDEVICE=pdfwrite",
                    "-sOutputFile=" + outputPdf,
                    "-f",
                    psPath
                });
            }

            return outputPdf;
        }

        // dummy method just to list other viewer properties and methods
        private void Other_Viewer_Methods()
        {
            // show first pdf page
            _viewer.ShowFirstPage();
            // show previous pdf page
            _viewer.ShowPreviousPage();
            // show next pdf page
            _viewer.ShowNextPage();
            // show last pdf page
            _viewer.ShowLastPage();
            // show page based on page number
            _viewer.ShowPage(6);
            // refresh current page / rasterize it again
            _viewer.RefreshPage();
            // zoom in
            _viewer.ZoomIn();
            // zoom out
            _viewer.ZoomOut();
            // get first page number
            int fpn = _viewer.FirstPageNumber;
            // get last page number
            int lpn = _viewer.LastPageNumber;
            // get current page number
            int cpn = _viewer.CurrentPageNumber;
            // gets or sets eps clip on or off
            bool epsClip = _viewer.EPSClip;
            // gets or sets graphics aplha bits
            int gab = _viewer.GraphicsAlphaBits;
            // gets or sets text aplha bits
            int gtb = _viewer.TextAlphaBits;
            // gets or sets progressive update on or off
            bool pu = _viewer.ProgressiveUpdate;
        }

    }
}
