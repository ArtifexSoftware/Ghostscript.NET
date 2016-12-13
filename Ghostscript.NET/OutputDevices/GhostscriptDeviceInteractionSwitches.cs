//
// GhostscriptDeviceInteractionSwitches.cs
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

namespace Ghostscript.NET
{
    public class GhostscriptDeviceInteractionSwitches
    {
        #region Batch

        /// <summary>
        /// Causes Ghostscript to exit after processing all files named on the command line, rather than going into an 
        /// interactive loop reading PostScript commands. Equivalent to putting -c quit at the end of the command line.
        /// </summary>
        [GhostscriptSwitch("-dBATCH")]
        public GhostscriptOptionalSwitch? Batch { get; set; }

        #endregion

        #region NoPagePrompt

        /// <summary>
        /// Disables only the prompt, but not the pause, at the end of each page. This may be useful on PC displays that
        /// get confused if a program attempts to write text to the console while the display is in a graphics mode.
        /// </summary>
        [GhostscriptSwitch("-dNOPAGEPROMPT")]
        public GhostscriptOptionalSwitch? NoPagePrompt { get; set; }

        #endregion

        #region NoPause

        /// <summary>
        /// Disables the prompt and pause at the end of each page. Normally one should use this (along with -dBATCH) when
        /// producing output on a printer or to a file; it also may be desirable for applications where another program is
        /// "driving" Ghostscript.
        /// </summary>
        [GhostscriptSwitch("-dNOPAUSE")]
        public GhostscriptOptionalSwitch? NoPause { get; set; }

        #endregion

        #region NoPrompt

        /// <summary>
        /// Disables the prompt printed by Ghostscript when it expects interactive input, as well as the end-of-page prompt
        /// (-dNOPAGEPROMPT). This allows piping input directly into Ghostscript, as long as the data doesn't refer to currentfile.
        /// </summary>
        [GhostscriptSwitch("-dNOPROMPT")]
        public GhostscriptOptionalSwitch? NoPrompt { get; set; }

        #endregion

        #region Quiet

        /// <summary>
        /// Suppresses routine information comments on standard output. This is currently necessary when redirecting 
        /// device output to standard output.
        /// </summary>
        [GhostscriptSwitch("-dQUIET")]
        public GhostscriptOptionalSwitch? Quiet { get; set; }

        #endregion

        #region ShortErrors

        /// <summary>
        /// Makes certain error and information messages more Adobe-compatible.
        /// </summary>
        [GhostscriptSwitch("-dSHORTERRORS")]
        public GhostscriptOptionalSwitch? ShortErrors { get; set; }

        #endregion
    }
}
