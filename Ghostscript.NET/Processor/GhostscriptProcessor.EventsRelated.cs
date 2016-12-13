//
// GhostscriptProcessor.EventsRelated.cs
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

namespace Ghostscript.NET.Processor
{

    #region Internal delegates

    internal delegate void StdInputEventHandler(out string input, int count);
    internal delegate void StdOutputEventHandler(string output);
    internal delegate void StdErrorEventHandler(string error);

    #endregion

    #region Public delegates

    public delegate void GhostscriptProcessorEventHandler(object sender, GhostscriptProcessorEventArgs e);
    public delegate void GhostscriptProcessorProcessingEventHandler(object sender, GhostscriptProcessorProcessingEventArgs e);
    public delegate void GhostscriptProcessorErrorEventHandler(object sender, GhostscriptProcessorErrorEventArgs e);

    #endregion

    #region GhostscriptProcessorEventArgs

    public class GhostscriptProcessorEventArgs : EventArgs
    {
        public GhostscriptProcessorEventArgs() { }
    }

    #endregion

    #region GhostscriptProcessorProcessingEventArgs

    public class GhostscriptProcessorProcessingEventArgs : EventArgs
    {
        private int _currentPage;
        private int _totalPages;

        internal GhostscriptProcessorProcessingEventArgs(int currentPage, int totalPages)
        {
            _currentPage = currentPage;
            _totalPages = totalPages;
        }

        public int CurrentPage
        {
            get { return _currentPage; }
        }

        public int TotalPages
        {
            get { return _totalPages; }
        }
    }

    #endregion

    #region GhostscriptProcessorErrorEventArgs

    public class GhostscriptProcessorErrorEventArgs : EventArgs
    {
        private string _errorMessage;

        public GhostscriptProcessorErrorEventArgs(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public string Message
        {
            get { return _errorMessage; }
        }
    }

    #endregion

}
