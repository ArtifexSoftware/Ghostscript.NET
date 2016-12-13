//
// ViewerSample.cs
// This file is part of Ghostscript.NET.Samples project
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
using System.Drawing;

// required Ghostscript.NET namespaces
using Ghostscript.NET;
using Ghostscript.NET.Viewer;

namespace Ghostscript.NET.Samples
{
    public class ViewerSample : ISample
    {
        private GhostscriptVersionInfo _lastInstalledVersion = null;
        private GhostscriptViewer _viewer = null;
        private Bitmap _pdfPage = null;

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

            // open PDF file using the last Ghostscript version. If you want to use
            // multiple viewers withing a single process then you need to pass 'true' 
            // value as the last parameter of the method below in order to tell the
            // viewer to load Ghostscript from the memory and not from the disk.
            _viewer.Open("E:\test\test.pdf",_lastInstalledVersion, false);
        }

        // this is the first raised event before PDF page starts rasterizing. 
        // this event is raised only once per page showing and it tells us 
        // the dimensions of the PDF page and gives us page image reference.
        void _viewer_DisplaySize(object sender, GhostscriptViewerViewEventArgs e)
        {
            // store PDF page image reference
            _pdfPage = e.Image;
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
            // memory and does not create new Bitmap instance.
        }

        // this is the last raised event after complete page is rasterized
        void _viewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            // complete PDF page is rasterized and we can update our PictureBox
            // once again by calling PictureBox.Invalidate() and PictureBox.Update()
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
            bool pu =_viewer.ProgressiveUpdate;
        }

    }
}

