//
// GhostscriptViewer.cs
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
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using Ghostscript.NET.Interpreter;

namespace Ghostscript.NET.Viewer
{
    public class GhostscriptViewer : IDisposable
    {
        #region Private variables

        private bool _disposed = false;
        private GhostscriptInterpreter _interpreter = null;
        private string _filePath = null;
        private GhostscriptViewerFormatHandler _formatHandler = null;
        private bool _progressiveUpdate = true;
        private int _progressiveUpdateInterval = 100;
        private GhostscriptStdIO _stdIoCallback = null;
        private int _zoom_xDpi = 96;
        private int _zoom_yDpi = 96;
        private bool _showPageAfterOpen = true;
        private FileCleanupHelper _fileCleanupHelper = new FileCleanupHelper();
        private int _graphicsAlphaBits = 4;
        private int _textAlphaBits = 4;
        private bool _epsClip = true;
        private List<string> _customSwitches = new List<string>();

        #endregion

        #region Public events

        public event GhostscriptViewerViewEventHandler DisplaySize;
        public event GhostscriptViewerViewEventHandler DisplayUpdate;
        public event GhostscriptViewerViewEventHandler DisplayPage;

        #region OnDisplaySize

        protected virtual void OnDisplaySize(GhostscriptViewerViewEventArgs e)
        {
            if (DisplaySize != null)
            {
                DisplaySize(this, e);
            }
        }

        #endregion

        #region OnDisplayUpdate

        protected virtual void OnDisplayUpdate(GhostscriptViewerViewEventArgs e)
        {
            if (DisplayUpdate != null)
            {
                DisplayUpdate(this, e);
            }
        }

        #endregion

        #region OnDisplayPage

        protected virtual void OnDisplayPage(GhostscriptViewerViewEventArgs e)
        {
            if (DisplayPage != null)
            {
                DisplayPage(this, e);
            }
        }

        #endregion

        #endregion

        #region Constructor

        public GhostscriptViewer()
        {

        }

        #endregion

        #region Destructor

        ~GhostscriptViewer()
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
                    if (_formatHandler != null)
                    {
                        _formatHandler.Dispose();
                        _formatHandler = null;
                    }

                    if (_interpreter != null)
                    {
                        _interpreter.Dispose();
                        _interpreter = null;
                    }
                }

                _fileCleanupHelper.Cleanup();

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Open - stream

        public void Open(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            string path = StreamHelper.WriteToTemporaryFile(stream);

            _fileCleanupHelper.Add(path);

            this.Open(path);
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

        public void Open(Stream stream, GhostscriptVersionInfo versionInfo, bool dllFromMemory)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo");
            }

            string path = StreamHelper.WriteToTemporaryFile(stream);

            _fileCleanupHelper.Add(path);

            this.Open(path, versionInfo, dllFromMemory);
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
                throw new ArgumentNullException("versionInfo");
            }

            this.Close();

            _filePath = path;

            _interpreter = new GhostscriptInterpreter(versionInfo, dllFromMemory);

            this.Open();
        }

        #endregion

        #region Open - stream, library

        public void Open(Stream stream, byte[] library)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            string path = StreamHelper.WriteToTemporaryFile(stream);

            _fileCleanupHelper.Add(path);

            this.Open(path, library);
        }

        #endregion

        #region Open - path, library

        public void Open(string path, byte[] library)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find input file.", path);
            }

            if (library == null)
            {
                throw new ArgumentNullException("library");
            }

            this.Close();

            _filePath = path;

            _interpreter = new GhostscriptInterpreter(library);

            this.Open();
        }

        #endregion

        #region Open - versionInfo, dllFromMemory

        public void Open(GhostscriptVersionInfo versionInfo, bool dllFromMemory)
        {
            if (versionInfo == null)
            {
                throw new ArgumentNullException("versionInfo");
            }

            this.Close();

            _filePath = string.Empty;

            _interpreter = new GhostscriptInterpreter(versionInfo, dllFromMemory);

            this.Open();
        }

        #endregion

        #region Open - library

        public void Open(byte[] library)
        {
            if (library == null)
            {
                throw new ArgumentNullException("library");
            }

            this.Close();

            _filePath = string.Empty;

            _interpreter = new GhostscriptInterpreter(library);

            this.Open();
        }

        #endregion

        #region Open

        private void Open()
        {
            string extension = Path.GetExtension(_filePath).ToLower();

            if (!string.IsNullOrWhiteSpace(_filePath) && string.IsNullOrWhiteSpace(extension))
            {
                using (FileStream srm = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    extension = StreamHelper.GetStreamExtension(srm);
                }
            }

            switch (extension)
            {
                case ".pdf":
                    {
                        _formatHandler = new GhostscriptViewerPdfFormatHandler(this);
                        break;
                    }
                case ".ps":
                    {
                        _formatHandler = new GhostscriptViewerPsFormatHandler(this);
                        break;
                    }
                case ".eps":
                    {
                        _formatHandler = new GhostscriptViewerEpsFormatHandler(this);
                        break;
                    }
                default:
                    {
                        _formatHandler = new GhostscriptViewerDefaultFormatHandler(this);
                        break;
                    }
            }

            _interpreter.Setup(new GhostscriptViewerStdIOHandler(this, _formatHandler), new GhostscriptViewerDisplayHandler(this));

            List<string> args = new List<string>();
            args.Add("-gsnet");
            args.Add("-sDEVICE=display");

            if (Environment.Is64BitProcess)
            {
                args.Add("-sDisplayHandle=0");
            }
            else
            {
                args.Add("-dDisplayHandle=0");
            }

            args.Add("-dDisplayFormat=" +
                        ((int)DISPLAY_FORMAT_COLOR.DISPLAY_COLORS_RGB |
                        (int)DISPLAY_FORMAT_ALPHA.DISPLAY_ALPHA_NONE |
                        (int)DISPLAY_FORMAT_DEPTH.DISPLAY_DEPTH_8 |
                        (int)DISPLAY_FORMAT_ENDIAN.DISPLAY_LITTLEENDIAN |
                        (int)DISPLAY_FORMAT_FIRSTROW.DISPLAY_BOTTOMFIRST).ToString());


            if (_interpreter.LibraryRevision > 950)
            {
                args.Add("--permit-file-read=" + _filePath);
            }

            args.Add("-dDOINTERPOLATE");
            args.Add("-dGridFitTT=0");

            // fixes bug: http://bugs.ghostscript.com/show_bug.cgi?id=695180
            if (_interpreter.LibraryRevision > 910)
            {
                args.Add("-dMaxBitmap=1g");
            }

            foreach(string customSwitch in _customSwitches)
            {
                args.Add(customSwitch);
            }

            _interpreter.InitArgs(args.ToArray());

            _formatHandler.Initialize();

            _formatHandler.Open(_filePath);

            if (_showPageAfterOpen)
            {
                this.ShowPage(_formatHandler.FirstPageNumber, true);
            }
        }

        #endregion

        #region Close

        public void Close()
        {
            if (_formatHandler != null)
            {
                _formatHandler = null;
            }

            if (_interpreter != null)
            {
                _interpreter.Dispose();
                _interpreter = null;
            }
        }

        #endregion

        #region AttachStdIO

        public void AttachStdIO(GhostscriptStdIO stdIoCallback)
        {
            _stdIoCallback = stdIoCallback;
        }

        #endregion

        #region ShowPage - pageNumber

        public void ShowPage(int pageNumber)
        {
            this.ShowPage(pageNumber, false);
        }

        #endregion

        #region ShowPage - pageNumber, refresh

        public void ShowPage(int pageNumber, bool refresh)
        {
            if (!this.IsEverythingInitialized)
                return;

            if (refresh == false && pageNumber == this.CurrentPageNumber)
                return;

            _formatHandler.InitPage(pageNumber);


            this.Interpreter.Run(
                            "%%BeginPageSetup\n" +
                            "<<\n");

            this.Interpreter.Run(string.Format("/HWResolution [{0} {1}]\n", _zoom_xDpi, _zoom_yDpi));

            GhostscriptRectangle mediaBox = _formatHandler.MediaBox;
            GhostscriptRectangle boundingBox = _formatHandler.BoundingBox;
            GhostscriptRectangle cropBox = _formatHandler.CropBox;

            float pageWidth = 0;
            float pageHeight = 0;

            if ((_formatHandler.GetType() == typeof(GhostscriptViewerEpsFormatHandler) && this.EPSClip && boundingBox != GhostscriptRectangle.Empty))
            {
                pageWidth = boundingBox.urx - boundingBox.llx;
                pageHeight = boundingBox.ury - boundingBox.lly;
            }
            else
            {
                if (cropBox != GhostscriptRectangle.Empty)
                {
                    pageWidth = cropBox.urx - cropBox.llx;
                    pageHeight = cropBox.ury - cropBox.lly;
                }
                else
                {
                    pageWidth = mediaBox.urx - mediaBox.llx;
                    pageHeight = mediaBox.ury - mediaBox.lly;
                }
            }

            pageWidth = Math.Abs(pageWidth);
            pageHeight = Math.Abs(pageHeight);

            if (pageWidth > 0 && pageHeight > 0)
            {
                this.Interpreter.Run(string.Format("/PageSize [{0} {1}]\n",
                                        pageWidth.ToString("0.00", CultureInfo.InvariantCulture),
                                        pageHeight.ToString("0.00", CultureInfo.InvariantCulture)));
            }

            if (cropBox != GhostscriptRectangle.Empty)
            {
                mediaBox = cropBox;
            }

            if (mediaBox == GhostscriptRectangle.Empty && boundingBox != GhostscriptRectangle.Empty)
            {
                mediaBox = boundingBox;
            }

            if (mediaBox != GhostscriptRectangle.Empty && _formatHandler.GetType() != typeof(GhostscriptViewerPsFormatHandler))
            {
                if (_formatHandler.PageOrientation == GhostscriptPageOrientation.Portrait)
                {
                    this.Interpreter.Run(string.Format("/PageOffset  [{0} {1}]\n",
                                            (-mediaBox.llx).ToString("0.00", CultureInfo.InvariantCulture),
                                            (-mediaBox.lly).ToString("0.00", CultureInfo.InvariantCulture)));
                }
                else if (_formatHandler.PageOrientation == GhostscriptPageOrientation.Landscape)
                {
                    this.Interpreter.Run(string.Format("/PageOffset  [{0} {1}]\n",
                                            (-mediaBox.lly).ToString("0.00", CultureInfo.InvariantCulture),
                                            (mediaBox.llx).ToString("0.00", CultureInfo.InvariantCulture)));
                }
                else if (_formatHandler.PageOrientation == GhostscriptPageOrientation.UpsideDown)
                {
                    this.Interpreter.Run(string.Format("/PageOffset  [{0} {1}]\n",
                                            (mediaBox.llx).ToString("0.00", CultureInfo.InvariantCulture),
                                            (mediaBox.lly).ToString("0.00", CultureInfo.InvariantCulture)));
                }
                else if (_formatHandler.PageOrientation == GhostscriptPageOrientation.Seascape)
                {
                    this.Interpreter.Run(string.Format("/PageOffset  [{0} {1}]\n",
                                            (mediaBox.lly).ToString("0.00", CultureInfo.InvariantCulture),
                                            (-mediaBox.llx).ToString("0.00", CultureInfo.InvariantCulture)));
                }
            }

            this.Interpreter.Run(string.Format("/GraphicsAlphaBits {0}\n", _graphicsAlphaBits));
            this.Interpreter.Run(string.Format("/TextAlphaBits {0}\n", _textAlphaBits));

            this.Interpreter.Run(">> setpagedevice\n");

            this.Interpreter.Run(
                            "%%EndPageSetup\n");

            _formatHandler.ShowPage(pageNumber);
        }

        #endregion

        #region ShowFirstPage

        public void ShowFirstPage()
        {
            if (this.CurrentPageNumber == this.FirstPageNumber)
                return;

            this.ShowPage(this.FirstPageNumber);
        }

        #endregion

        #region ShowNextPage

        public void ShowNextPage()
        {
            if (this.CurrentPageNumber + 1 <= this.LastPageNumber)
            {
                this.ShowPage(this.CurrentPageNumber + 1);
            }
        }

        #endregion

        #region ShowPreviousPage

        public void ShowPreviousPage()
        {
            if (this.CurrentPageNumber - 1 >= this.FirstPageNumber)
            {
                this.ShowPage(this.CurrentPageNumber - 1);
            }
        }

        #endregion

        #region ShowLastPage

        public void ShowLastPage()
        {
            if (this.CurrentPageNumber == this.LastPageNumber)
                return;

            this.ShowPage(this.LastPageNumber);
        }

        #endregion

        #region RefreshPage

        public void RefreshPage()
        {
            if (this.IsEverythingInitialized)
            {
                this.ShowPage(this.CurrentPageNumber, true);
            }
        }

        #endregion

        #region IsPageNumberValid

        public bool IsPageNumberValid(int pageNumber)
        {
            if (pageNumber >= this.FirstPageNumber && pageNumber <= this.LastPageNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Zoom

        public bool Zoom(float scale, bool test = false)
        {
            int tmpZoopX = (int)(_zoom_xDpi * scale + 0.5);
            int tmpZoomY = (int)(_zoom_yDpi * scale + 0.5);

            if (tmpZoopX < 39)
                return false;

            if (tmpZoopX > 496)
                return false;

            if (!test)
            {
                _zoom_xDpi = tmpZoopX;
                _zoom_yDpi = tmpZoomY;
            }

            return true;
        }

        #endregion

        #region ZoomIn

        public void ZoomIn()
        {
            if (this.IsEverythingInitialized)
            {
                this.Zoom(1.2f, false);
                this.RefreshPage();
            }
        }

        #endregion

        #region ZoomOut

        public void ZoomOut()
        {
            if (this.IsEverythingInitialized)
            {
                this.Zoom(0.8333333f, false);
                this.RefreshPage();
            }
        }

        #endregion

        #region SaveState

        public GhostscriptViewerState SaveState()
        {
            GhostscriptViewerState state = new GhostscriptViewerState();
            state.XDpi = _zoom_xDpi;
            state.YDpi = _zoom_yDpi;
            state.CurrentPage = _formatHandler.CurrentPageNumber;
            state.ProgressiveUpdate = _progressiveUpdate;
            return state;
        }

        #endregion

        #region RestoreState

        public void RestoreState(GhostscriptViewerState state)
        {
            _zoom_xDpi = state.XDpi;
            _zoom_yDpi = state.YDpi;
            _formatHandler.CurrentPageNumber = state.CurrentPage;
            _progressiveUpdate = state.ProgressiveUpdate;
        }

        #endregion

        #region Internal methods

        #region StdInput

        internal void StdInput(out string input, int count)
        {
            input = null;

            if (_stdIoCallback != null)
            {
                _stdIoCallback.StdIn(out input, count);
            }
        }

        #endregion

        #region StdOutput

        internal void StdOutput(string message)
        {
            if (_stdIoCallback != null)
            {
                _stdIoCallback.StdOut(message);
            }
        }

        #endregion

        #region StdError

        internal void StdError(string message)
        {
            if (_stdIoCallback != null)
            {
                _stdIoCallback.StdError(message);
            }
        }

        #endregion

        #region RaiseDisplaySize

        internal void RaiseDisplaySize(GhostscriptViewerViewEventArgs e)
        {
            this.OnDisplaySize(e);
        }

        #endregion

        #region RaiseDisplayPage

        internal void RaiseDisplayPage(GhostscriptViewerViewEventArgs e)
        {
            this.OnDisplayPage(e);
        }

        #endregion

        #region RaiseDisplayUpdate

        internal void RaiseDisplayUpdate(GhostscriptViewerViewEventArgs e)
        {
            this.OnDisplayUpdate(e);
        }

        #endregion

        #region ZoomXDpi

        internal int ZoomXDpi
        {
            get { return _zoom_xDpi; }
            set { _zoom_xDpi = value; }
        }

        #endregion

        #region ZoomYDpi

        internal int ZoomYDpi
        {
            get { return _zoom_yDpi; }
            set { _zoom_yDpi = value; }
        }

        #endregion

        #endregion

        #region Public properties

        #region Interpreter

        public GhostscriptInterpreter Interpreter
        {
            get { return _interpreter; }
        }

        #endregion

        #region IsEverythingInitialized

        public bool IsEverythingInitialized
        {
            get { return _formatHandler != null; }
        }

        #endregion

        #region FilePath

        public string FilePath
        {
            get
            {
                if (this.IsEverythingInitialized)
                {
                    return _filePath;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region CurrentPageNumber

        public int CurrentPageNumber
        {
            get 
            {
                if (this.IsEverythingInitialized)
                {
                    return _formatHandler.CurrentPageNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region FirstPageNumber

        public int FirstPageNumber
        {
            get 
            {
                if (this.IsEverythingInitialized)
                {
                    return _formatHandler.FirstPageNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region LastPageNumber

        public int LastPageNumber
        {
            get 
            {
                if (this.IsEverythingInitialized)
                {
                    return _formatHandler.LastPageNumber;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region ProgressiveUpdate

        public bool ProgressiveUpdate
        {
            get
            {
                return _progressiveUpdate;
            }
            set
            {
                _progressiveUpdate = value;
            }
        }

        #endregion

        #region ProgressiveUpdateInterval

        public int ProgressiveUpdateInterval
        {
            get { return _progressiveUpdateInterval; }
            set { _progressiveUpdateInterval = value; }
        }

        #endregion

        #region CanShowFirstPage

        public bool CanShowFirstPage
        {
            get
            {
                return this.CurrentPageNumber != this.FirstPageNumber;
            }
        }

        #endregion

        #region CanShowPreviousPage

        public bool CanShowPreviousPage
        {
            get
            {
                return this.CurrentPageNumber > this.FirstPageNumber;
            }
        }

        #endregion
        
        #region CanShowNextPage

        public bool CanShowNextPage
        {
            get
            {
                return this.CurrentPageNumber < this.LastPageNumber;
            }
        }

        #endregion

        #region CanShowLastPage

        public bool CanShowLastPage
        {
            get
            {
                return this.CurrentPageNumber != this.LastPageNumber;
            }
        }

        #endregion

        #region CanZoomIn

        public bool CanZoomIn
        {
            get
            {
                return this.Zoom(1.2f, true);
            }
        }

        #endregion

        #region CanZoomOut

        public bool CanZoomOut
        {
            get
            {
                return this.Zoom(0.8333333f, true);
            }
        }

        #endregion

        #region GraphicsAlphaBits

        public int GraphicsAlphaBits
        {
            get { return _graphicsAlphaBits; }
            set { _graphicsAlphaBits = value; }
        }

        #endregion

        #region TextAlphaBits

        public int TextAlphaBits
        {
            get { return _textAlphaBits; }
            set { _textAlphaBits = value; }
        }

        #endregion

        #region EPSClip

        public bool EPSClip
        {
            get
            {
                return _epsClip;
            }
            set
            {
                _epsClip = value;
            }
        }

        #endregion

        #region CurrentPageOrientation

        public GhostscriptPageOrientation CurrentPageOrientation
        {
            get
            {
                if (this.IsEverythingInitialized)
                {
                    return _formatHandler.PageOrientation;
                }
                else
                {
                    return GhostscriptPageOrientation.Landscape;
                }
            }
        }

        #endregion

        #region CustomSwitches

        public List<string> CustomSwitches
        {
            get
            {
                return _customSwitches;
            }
            set
            {
                _customSwitches = value;
            }
        }

        #endregion

        #region DPI

        public int Dpi
        {
            get { return ZoomXDpi; }
            set
            {
                ZoomXDpi = value;
                ZoomYDpi = value;
            }
        }

        #endregion

        #endregion

        #region Internal properties

        #region ShowPageAfterOpen

        public bool ShowPageAfterOpen
        {
            get { return _showPageAfterOpen; }
            set { _showPageAfterOpen = value; }
        }

        #endregion

        #endregion

        #region Internal properties

        #region FormatHandler

        internal GhostscriptViewerFormatHandler FormatHandler
        {
            get { return _formatHandler; }
        }

        #endregion

        #endregion
    }
}
