//
// GhostscriptNativeKind.cs
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

namespace Ghostscript.NET
{
    /// <summary>
    /// Identifies which native interpreter a loaded library is.
    /// </summary>
    public enum GhostscriptNativeKind
    {
        /// <summary>
        /// Standard Ghostscript library (<c>gsdll64.dll</c> / <c>libgs.so</c>).
        /// PDF, PostScript, and EPS only.
        /// </summary>
        Ghostscript = 0,

        /// <summary>
        /// GhostPDL library (<c>gpdldll64.dll</c> / <c>libgpdl.so</c>).
        /// Same <c>gsapi_*</c> surface as Ghostscript, plus Office (SmartOffice),
        /// PCL, XPS, and additional image languages.
        /// </summary>
        GhostPdl = 1
    }
}
