//
// GhostscriptViewerStdIOHandler.cs
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
using System.Text;

namespace Ghostscript.NET.Viewer
{
    internal class GhostscriptViewerStdIOHandler : GhostscriptStdIO
    {

        #region Private variables

        private GhostscriptViewer _viewer;
        private GhostscriptViewerFormatHandler _formatHandler;
        private StringBuilder _outputMessages = new StringBuilder();
        private StringBuilder _errorMessages = new StringBuilder();

        #endregion

        #region Constructor

        public GhostscriptViewerStdIOHandler(GhostscriptViewer viewer, GhostscriptViewerFormatHandler formatHandler) : base(true, true, true)
        {
            _viewer = viewer;
            _formatHandler = formatHandler;
        }

        #endregion

        #region StdIn

        public override void StdIn(out string input, int count)
        {
            input = string.Empty;

            if (_formatHandler != null)
            {
                _formatHandler.StdInput(out input, count);
            }
        }

        #endregion

        #region StdOut

        public override void StdOut(string output)
        {
            lock (_outputMessages)
            {
                _outputMessages.Append(output);

                int rIndex = _outputMessages.ToString().IndexOf("\r\n");

                while (rIndex > -1)
                {
                    string line = _outputMessages.ToString().Substring(0, rIndex);
                    _outputMessages = _outputMessages.Remove(0, rIndex + 2);

                    _viewer.StdOutput(line);

                    if (_formatHandler != null)
                    {
                        _formatHandler.StdOutput(line);
                    }

                    rIndex = _outputMessages.ToString().IndexOf("\r\n");
                }
            }
        }

        #endregion

        #region StdError

        public override void StdError(string error)
        {
            lock (_errorMessages)
            {
                _errorMessages.Append(error);

                int rIndex = _errorMessages.ToString().IndexOf("\r\n");

                while (rIndex > -1)
                {
                    string line = _errorMessages.ToString().Substring(0, rIndex);
                    _errorMessages = _errorMessages.Remove(0, rIndex + 2);

                    _viewer.StdError(line);

                    if (_formatHandler != null)
                    {
                        _formatHandler.StdError(line);
                    }

                    rIndex = _errorMessages.ToString().IndexOf("\r\n");
                }
            }
        }

        #endregion

    }
}
