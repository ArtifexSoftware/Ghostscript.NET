//
// GhostscriptStdIO.cs
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
using System.Runtime.InteropServices;

namespace Ghostscript.NET
{
    /// <summary>
    /// Represents a base Ghostscript standard input output handler.
    /// </summary>
    public abstract class GhostscriptStdIO
    {

        #region Internal variables

        internal gsapi_stdio_callback _std_in = null;
        internal gsapi_stdio_callback _std_out = null;
        internal gsapi_stdio_callback _std_err = null;

        #endregion

        #region Private variables

        private StringBuilder _input = new StringBuilder();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptStdIO class.
        /// </summary>
        /// <param name="handleStdIn">Whether or not to handle Ghostscript standard input.</param>
        /// <param name="handleStdOut">Whether or not to handle Ghostscript standard output.</param>
        /// <param name="handleStdErr">Whether or not to handle Ghostscript standard errors.</param>
        public GhostscriptStdIO(bool handleStdIn, bool handleStdOut, bool handleStdErr)
        {
            // check if we need to handle standard input
            if (handleStdIn)
            {
                // attach standard input handler
                _std_in = new gsapi_stdio_callback(gs_std_in);
            }

            // check if we need to handle standard output
            if (handleStdOut)
            {
                // attach standard output handler
                _std_out = new gsapi_stdio_callback(gs_std_out);
            }

            // check if we need to handle errors
            if (handleStdErr)
            {
                // attach error handler
                _std_err = new gsapi_stdio_callback(gs_std_err);
            }
        }

        #endregion

        #region gs_std_in
        
        /// <summary>
        /// Standard input handler.
        /// </summary>
        /// <param name="handle">Standard input handle.</param>
        /// <param name="pointer">Pointer to a memroy block.</param>
        /// <param name="count">Number of bytes that standard input expects.</param>
        /// <returns>Number of bytes returned.</returns>
        private int gs_std_in(IntPtr handle, IntPtr pointer, int count)
        {
            // check if we have anything in the local input cache
            if (_input.Length == 0)
            {
                string input = string.Empty;

                // ask handler owner for the input data
                this.StdIn(out input, count);

                // check if we have input
                if (!string.IsNullOrEmpty(input))
                {
                    // add the input to the local cache
                    _input.Append(input);
                }
                else
                {
                    // we don't have any input
                    return 0;
                }
            }
           
            // check if the stdin expects more data than we have at the moment
            if (count > _input.Length)
            {
                // locally set the count to a length of the data we have
                count = _input.Length;
            }

            int position = 0;

            // loop through data
            while (position < count)
            {
                // get single character
                char c = _input[position];

                // write single character to the expected input memory block
                Marshal.WriteByte(pointer, position, (byte)c);

                position++;

                // break if we got to the new line
                if (c == '\n')
                {
                    break;
                }
            }

            // remove written data out from the cached input
            _input = _input.Remove(0, position);

            // return number of bytes written
            return position;
        }

        #endregion

        #region gs_std_out

        /// <summary>
        /// Handles standard output.
        /// </summary>
        /// <param name="handle">Standard output handle.</param>
        /// <param name="pointer">Pointer to a memroy block.</param>
        /// <param name="count">Number of bytes that standard output writes.</param>
        /// <returns>Number of bytes read.</returns>
        private int gs_std_out(IntPtr handle, IntPtr pointer, int count)
        {
            // read out the standard output data
            string output = Marshal.PtrToStringAnsi(pointer, count);

            // replace line feeds with the standard windows new line
            output = output.Replace("\n", "\r\n");

            // send read out data to the handler owner
            this.StdOut(output);

            // return number of bytes read
            return count;
        }

        #endregion

        #region gs_std_err

        /// <summary>
        /// Handles errors.
        /// </summary>
        /// <param name="handle">Errors handle.</param>
        /// <param name="pointer">Pointer to a memory block.</param>
        /// <param name="count">Number of bytes standard error writes.</param>
        /// <returns>Number of bytes read.</returns>
        private int gs_std_err(IntPtr handle, IntPtr pointer, int count)
        {
            // read out the standard error data
            string errors = Marshal.PtrToStringAnsi(pointer, count);

            // replace line feeds with the standard windows new line
            errors = errors.Replace("\n", "\r\n");

            // send read out data to the handler owner
            this.StdError(errors);

            // return number of bytes read
            return count;
        }

        #endregion

        #region Abstract functions

        /// <summary>
        /// Abstract standard input method.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <param name="count">Expected size of the input data.</param>
        public abstract void StdIn(out string input, int count);

        /// <summary>
        /// Abstract standard output method.
        /// </summary>
        /// <param name="output">Output data.</param>
        public abstract void StdOut(string output);

        /// <summary>
        /// Abstract standard error method.
        /// </summary>
        /// <param name="error">Error data.</param>
        public abstract void StdError(string error);

        #endregion

    }
}
