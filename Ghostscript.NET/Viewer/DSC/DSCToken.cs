//
// DSCToken.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Author ported some parts of this code from GSView. 
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

namespace Ghostscript.NET.Viewer.DSC
{

    internal class DSCToken
    {

        #region Private variables

        private long _startPosition;
        private long _length;
        private string _text;
        private DSCTokenEnding _ending;

        #endregion

        #region StartPosition

        public long StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        #endregion

        #region Length

        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }

        #endregion

        #region Text

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region Ending

        public DSCTokenEnding Ending
        {
            get { return _ending; }
            set { _ending = value; }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return _text;
        }

        #endregion

    }
}
