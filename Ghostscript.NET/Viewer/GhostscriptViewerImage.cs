//
// GhostscriptViewerImage.cs
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
using System.Drawing.Imaging;

namespace Ghostscript.NET.Viewer
{
    public class GhostscriptViewerImage : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private int _width;
        private int _height;
        private Rectangle _rect;
        private int _stride;
        private Bitmap _bitmap;
        private BitmapData _bitmapData;

        #endregion

        #region Constructor

        public GhostscriptViewerImage(int width, int height, int stride, PixelFormat format)
        {
            _width = width;
            _height = height;
            _stride = stride;

            _rect = new Rectangle(0, 0, _width, _height);

            _bitmap = new Bitmap(width, height, format);
        }

        #endregion

        #region Destructor

        ~GhostscriptViewerImage()
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
                    if (_bitmapData != null)
                    {
                        this.Unlock();
                    }

                    _bitmap.Dispose();
                    _bitmap = null;
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Create

        public static GhostscriptViewerImage Create(int width, int height, int stride, PixelFormat format)
        {
            GhostscriptViewerImage gvi = new GhostscriptViewerImage(width, height, stride, format);
            return gvi;
        }

        #endregion

        #region Lock

        internal void Lock()
        {
            if (_bitmapData == null)
            {
                _bitmapData = _bitmap.LockBits(_rect, ImageLockMode.WriteOnly, _bitmap.PixelFormat);
            }
        }

        #endregion

        #region Scan0

        internal IntPtr Scan0
        {
            get { return _bitmapData.Scan0; }
        }

        #endregion

        #region Stride

        public int Stride
        {
            get { return _bitmapData.Stride; }
        }

        #endregion

        #region Unlock

        internal void Unlock()
        {
            if (_bitmapData != null)
            {
                _bitmap.UnlockBits(_bitmapData);
                _bitmapData = null;
            }
        }

        #endregion

        #region Width

        public int Width
        {
            get { return _width; }
        }

        #endregion

        #region Height

        public int Height
        {
            get { return _height; }
        }

        #endregion

        #region Bitmap

        public Bitmap @Bitmap
        {
            get { return _bitmap; }
        }

        #endregion

    }
}
