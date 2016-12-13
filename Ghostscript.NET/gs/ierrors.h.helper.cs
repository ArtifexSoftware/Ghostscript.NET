//
// ierrors.h.helper.cs
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
using System.Collections.Generic;

namespace Ghostscript.NET
{
    public partial class ierrors
    {
        public static bool IsError(int code)
        {
            return code < 0;
        }

        public static bool IsErrorIgnoreQuit(int code)
        {
            return (code < 0) && (code != ierrors.e_Quit);
        }

        public static bool IsFatalIgnoreNeedInput(int code)
        {
            return (code <= ierrors.e_Fatal) && (code != ierrors.e_NeedInput);
        }

        public static bool IsInterrupt(int ecode)
        {
            return ((ecode) == e_interrupt || (ecode) == e_timeout);
        }

        public static bool IsFatal(int code)
        {
            return code <= ierrors.e_Fatal;
        }

        /// <summary>
        /// Returns error name.
        /// </summary>
        /// <param name="returnCode">Return code from the Ghostscript.</param>
        /// <returns>Error name.</returns>
        public static string GetErrorName(int code)
        {
            int errorNameIndex = ~code + 1;
            return ERROR_NAMES[errorNameIndex];
        }
    }
}
