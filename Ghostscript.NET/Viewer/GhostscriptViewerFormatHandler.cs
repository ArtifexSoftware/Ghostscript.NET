//
// GhostscriptViewerFormatHandler.cs
// This file is part of Ghostscript.NET library
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
using System.Drawing;

namespace Ghostscript.NET.Viewer
{
    internal abstract class GhostscriptViewerFormatHandler : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private GhostscriptViewer _viewer = null;
        private int _firstPageNumber;
        private int _lastPageNumber;
        private int _currentPageNumber = 1;
        private GhostscriptRectangle _mediaBox = GhostscriptRectangle.Empty;
        private GhostscriptRectangle _boundingBox = GhostscriptRectangle.Empty;
        private GhostscriptRectangle _cropBox = GhostscriptRectangle.Empty;
        private GhostscriptPageOrientation _pageOrientation = GhostscriptPageOrientation.Portrait;
        private bool _showPagePostScriptCommandInvoked = false;

        #endregion

        #region Constructor

        public GhostscriptViewerFormatHandler(GhostscriptViewer viewer)
        {
            _viewer = viewer;
        }

        #endregion

        #region Destructor

        ~GhostscriptViewerFormatHandler()
        {
            this.Dispose(false);
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

                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Abstract methods

        public abstract void Initialize();
        public abstract void Open(string filePath);
        public abstract void StdInput(out string input, int count);
        public abstract void StdOutput(string message);
        public abstract void StdError(string message);
        public abstract void InitPage(int pageNumber);
        public abstract void ShowPage(int pageNumber);

        #endregion

        #region Execute

        public int Execute(string str)
        {
            return _viewer.Interpreter.Run(str);
        }

        #endregion

        #region Execute

        internal int Execute(IntPtr str)
        {
            return _viewer.Interpreter.Run(str);
        }

        #endregion

        #region Viewer

        public GhostscriptViewer Viewer
        {
            get { return _viewer; }
        }

        #endregion

        #region FirstPageNumber

        public int FirstPageNumber
        {
            get { return _firstPageNumber; }
            set { _firstPageNumber = value; }
        }

        #endregion

        #region LastPageNumber

        public int LastPageNumber
        {
            get { return _lastPageNumber; }
            set { _lastPageNumber = value; }
        }

        #endregion

        #region CurrentPageNumber

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            internal set { _currentPageNumber = value; }
        }

        #endregion

        #region MediaBox

        public GhostscriptRectangle MediaBox
        {
            get { return _mediaBox; }
            set { _mediaBox = value; }
        }

        #endregion

        #region BoundingBox

        public GhostscriptRectangle BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }

        #endregion

        #region CropBox

        public GhostscriptRectangle CropBox
        {
            get { return _cropBox; }
            set { _cropBox = value; }
        }

        #endregion

        #region IsMediaBoxSet

        public bool IsMediaBoxSet
        {
            get { return _mediaBox != GhostscriptRectangle.Empty; }
        }

        #endregion

        #region IsBoundingBoxSet

        public bool IsBoundingBoxSet
        {
            get { return _boundingBox != GhostscriptRectangle.Empty; }
        }

        #endregion

        #region IsCropBoxSet

        public bool IsCropBoxSet
        {
            get { return _cropBox != GhostscriptRectangle.Empty; }
        }

        #endregion

        #region PageOrientation

        public GhostscriptPageOrientation PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }

        #endregion

        #region ShowPageInvoked

        internal bool ShowPagePostScriptCommandInvoked
        {
            get
            {
                return _showPagePostScriptCommandInvoked;
            }
            set
            {
                _showPagePostScriptCommandInvoked = value;
            }
        }

        #endregion

    }
}
