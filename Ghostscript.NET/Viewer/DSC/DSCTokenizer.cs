//
// DSCTokenizer.cs
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
using System.IO;
using System.Text;

namespace Ghostscript.NET.Viewer.DSC
{
    internal class DSCTokenizer : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private Stream _stream;
        private BufferedStream _bufferedStream;
        private bool _initiallyOwned = false;
        private bool _isUnicode = false;
        private bool _isLittleEndian = BitConverter.IsLittleEndian;

        #endregion

        #region Constructor - path

        public DSCTokenizer(string path)
        {
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _initiallyOwned = true;
            _bufferedStream = new BufferedStream(_stream);
        }

        #endregion

        #region Constructor - stream

        public DSCTokenizer(Stream stream, bool isUnicode, bool isLittleEndian)
        {
            _stream = stream;
            _bufferedStream = new BufferedStream(_stream);
            _isUnicode = isUnicode;
            _isLittleEndian = isLittleEndian;
        }

        #endregion

        #region Destructor

        ~DSCTokenizer()
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
                    _bufferedStream.Close();
                    _bufferedStream.Dispose();
                    _bufferedStream = null;

                    if (_initiallyOwned)
                    {
                        _stream.Close();
                        _stream.Dispose();
                    }

                    _stream = null;
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region GetNextDSCKeywordToken

        public DSCToken GetNextDSCKeywordToken()
        {
            int c;

            while ((c = this.ReadChar()) > -1)
            {
                if (c == '%')
                {
                    if (this.ReadChar() == '%')
                    {
                        return this.ReadUntil("%%", DSCTokenEnding.Whitespace | DSCTokenEnding.LineEnd);
                    }
                }
            }

            return null;
        }

        #endregion

        #region GetNextDSCValueToken

        public DSCToken GetNextDSCValueToken(DSCTokenEnding end)
        {
            return this.ReadUntil(string.Empty, end);
        }

        #endregion

        #region ReadUntil

        private DSCToken ReadUntil(string prefix, DSCTokenEnding end)
        {
            int c;
            DSCToken token = new DSCToken();
            token.StartPosition = _bufferedStream.Position - prefix.Length;

            StringBuilder text = new StringBuilder(prefix, 64);

            int lastAppendedChar = 0;

            while ((c = this.ReadChar()) > -1)
            {
                if (c == '\n' && (end & DSCTokenEnding.LineEnd) == DSCTokenEnding.LineEnd)
                {
                    token.Length = _bufferedStream.Position - 1 - token.StartPosition;
                    token.Text = text.ToString().Trim();
                    token.Ending = DSCTokenEnding.LineEnd;
                    return token;
                }
                else if (c == '\r' && this.ReadChar() == '\n' && (end & DSCTokenEnding.LineEnd) == DSCTokenEnding.LineEnd)
                {
                    token.Length = _bufferedStream.Position - 2 - token.StartPosition;
                    token.Text = text.ToString().Trim();
                    token.Ending = DSCTokenEnding.LineEnd;
                    return token;
                }
                else if (c == ' ' && text.Length > 0 && lastAppendedChar != ' ' && (end & DSCTokenEnding.Whitespace) == DSCTokenEnding.Whitespace)
                {
                    token.Length = _bufferedStream.Position - 1 - token.StartPosition;
                    token.Text = text.ToString().Trim();
                    token.Ending = DSCTokenEnding.Whitespace;
                    return token;
                }
                else
                {
                    text.Append((char)c);
                    lastAppendedChar = c;
                }
            }

            return null;
        }

        #endregion

        #region ReadContent

        public string ReadContent(int start, int count)
        {
            long bkpPos = _bufferedStream.Position;

            _bufferedStream.Seek(start, SeekOrigin.Begin);
            byte[] buffer = new byte[count];
            int readCount = _bufferedStream.Read(buffer, 0, count);

            _bufferedStream.Seek(bkpPos, SeekOrigin.Begin);

            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        #endregion

        #region FileSize

        public long FileSize
        {
            get { return _bufferedStream.Length; }
        }

        #endregion

        #region ReadByte

        private int ReadChar()
        {
            if (_bufferedStream.Position == _bufferedStream.Length)
            {
                return -1;
            }

            if (_isUnicode)
            { 
                byte[] b = new byte[2];

                _bufferedStream.Read(b, 0, 2);

                if(_isLittleEndian)
                { 
                    return (int)(b[0] | b[1] << 8);
                }
                else
                {
                    return (int)(b[0] << 8 | b[1]);
                }
            }
            else
            {
                return _bufferedStream.ReadByte();
            }
        }

        #endregion
    }

}
