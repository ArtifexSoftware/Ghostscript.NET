//
// GhostscriptRasterizer.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2014 by Josip Habjan. All rights reserved.
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
using System.Drawing;
using Ghostscript.NET.Viewer;
using System.IO;

namespace Ghostscript.NET.Rasterizer
{
    public class GhostscriptRasterizer : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private GhostscriptViewer _viewer;
        private Image _lastRasterizedImage = null;
        private GhostscriptViewerState _gsViewState;

        #endregion

        #region Constructor

        public GhostscriptRasterizer()
        {
            _viewer = new GhostscriptViewer();
            _viewer.ShowPageAfterOpen = false;
            _viewer.ProgressiveUpdate = false;
            _viewer.DisplayPage += new GhostscriptViewerViewEventHandler(_viewer_DisplayPage);
        }

        #endregion

        #region Constructor - viewerInstance

        public GhostscriptRasterizer(GhostscriptViewer viewerInstance)
        {
            if (viewerInstance == null)
            {
                throw new ArgumentNullException("viewerInstance", "Cannot be null.");
            }

            _viewer = viewerInstance;
            _gsViewState = _viewer.SaveState();
            _viewer.ProgressiveUpdate = false;
            _viewer.DisplayPage += new GhostscriptViewerViewEventHandler(_viewer_DisplayPage);
        }

        #endregion

        #region Destructor

        ~GhostscriptRasterizer()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_gsViewState == null)
                    {
                        if (_viewer != null)
                        {
                            _viewer.Dispose();
                            _viewer = null;
                        }
                    }
                    else
                    {
                        _viewer.RestoreState(_gsViewState);
                    }
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Open - stream

        public void Open(MemoryStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Cannot be null.");
            }

            this.Open(stream, GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL), false);
        }

        #endregion

        #region Open - path

        public void Open(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find input file.", path);
            }

            this.Open(path, GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL), false);
        }

        #endregion

        #region Open - stream, versionInfo, dllFromMemory

        public void Open(MemoryStream stream, GhostscriptVersionInfo versionInfo, bool dllFromMemory)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Cannot be null.");
            }

            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo", "Cannot be null.");
            }

            if (_gsViewState == null)
            {
                _viewer.Open(stream, versionInfo, dllFromMemory);
            }
        }

        #endregion

        #region Open - path, versionInfo, dllFromMemory

        public void Open(string path, GhostscriptVersionInfo versionInfo, bool dllFromMemory)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find input file.", path);
            }

            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo", "Cannot be null.");
            }

            if (_gsViewState == null)
            {
                _viewer.Open(path, versionInfo, dllFromMemory);
            }
        }

        #endregion

        #region Open - stream, gsDll

        public void Open(MemoryStream stream, byte[] gsDll)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "Cannot be null.");
            }

            if (gsDll == null)
            {
                throw new ArgumentNullException("gsDll", "Cannot be null.");
            }

            if (_gsViewState == null)
            {
                _viewer.Open(stream, gsDll);
            }
        }

        #endregion

        #region Open - path, gsDll

        public void Open(string path, byte[] gsDll)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find input file.", path);
            }

            if (gsDll == null)
            {
                throw new ArgumentNullException("gsDll", "Cannot be null.");
            }

            if (_gsViewState == null)
            {
                _viewer.Open(path, gsDll);
            }
        }

        #endregion

        #region Close

        public void Close()
        {
            if (_gsViewState == null)
            {
                _viewer.Close();
            }
        }

        #endregion

        #region PageCount

        public int PageCount
        {
            get { return _viewer.LastPageNumber; }
        }

        #endregion

        #region GetPage

        public Image GetPage(int xDpi, int yDpi, int pageNumber)
        {
            _viewer.ZoomXDpi = xDpi;
            _viewer.ZoomYDpi = yDpi;
            _viewer.ShowPage(pageNumber, true);
            return _lastRasterizedImage;
        }

        #endregion

        #region _viewer_DisplayPage

        void _viewer_DisplayPage(object sender, GhostscriptViewerViewEventArgs e)
        {
            _lastRasterizedImage = e.Image;
        }

        #endregion

        #region GraphicsAlphaBits

        public int GraphicsAlphaBits
        {
            get { return _viewer.GraphicsAlphaBits; }
            set { _viewer.GraphicsAlphaBits = value; }
        }

        #endregion

        #region TextAlphaBits

        public int TextAlphaBits
        {
            get { return _viewer.TextAlphaBits; }
            set { _viewer.TextAlphaBits = value; }
        }

        #endregion

        #region EPSClip

        public bool EPSClip
        {
            get
            {
                return _viewer.EPSClip;
            }
            set
            {
                _viewer.EPSClip = value;
            }
        }

        #endregion

    }
}
