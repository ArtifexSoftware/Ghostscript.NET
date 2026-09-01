//
// GhostscriptPdlLibraryNotFoundException.cs
// This file is part of Ghostscript.NET library
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
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

namespace Ghostscript.NET
{
    /// <summary>
    /// Thrown when an Office file is processed but no GhostPDL native library was found.
    /// </summary>
    public class GhostscriptPdlLibraryNotFoundException : GhostscriptException
    {
        /// <summary>
        /// Creates an exception describing how to locate GhostPDL.
        /// </summary>
        public GhostscriptPdlLibraryNotFoundException()
            : base(BuildMessage(), -1002) { }

        /// <summary>
        /// Creates an exception with a custom message.
        /// </summary>
        public GhostscriptPdlLibraryNotFoundException(string message)
            : base(message, -1002) { }

        private static string BuildMessage()
        {
            string bitness = Environment.Is64BitProcess ? "64-bit" : "32-bit";
            string dllName = Environment.Is64BitProcess ? "gpdldll64.dll" : "gpdldll32.dll";
            return
                GhostscriptOffice.CommercialLicenseRequiredMessage +
                " If you already have that license, copy the " + bitness + " GhostPDL library (" + dllName + ") " +
                "from Ghostscript.NET.Office into your application folder, set GHOSTPDL_DLL to its full path, " +
                "or pass a GhostscriptVersionInfo that points at GhostPDL.";
        }
    }
}
