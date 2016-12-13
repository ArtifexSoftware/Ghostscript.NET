//
// ierrors.h.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan= habjan@gmail.com, http://www.linkedin.com/in/habjan; 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Author ported parts of this code from AFPL Ghostscript. 
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files= the
// "Software";, to deal in the Software without restriction, including
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
        static ierrors()
        {
            ERROR_NAMES.Add("not error");
            ERROR_NAMES.AddRange(LEVEL1_ERROR_NAMES);
            ERROR_NAMES.AddRange(LEVEL2_ERROR_NAMES);
            ERROR_NAMES.AddRange(DPS_ERROR_NAMES);
        }

        // PostScript Level 1 errors

        public const int e_unknownerror = -1;	/* unknown error */
        public const int e_dictfull = -2;
        public const int e_dictstackoverflow = -3;
        public const int e_dictstackunderflow = -4;
        public const int e_execstackoverflow = -5;
        public const int e_interrupt = -6;
        public const int e_invalidaccess = -7;
        public const int e_invalidexit = -8;
        public const int e_invalidfileaccess = -9;
        public const int e_invalidfont = -10;
        public const int e_invalidrestore = -11;
        public const int e_ioerror = -12;
        public const int e_limitcheck = -13;
        public const int e_nocurrentpoint = -14;
        public const int e_rangecheck = -15;
        public const int e_stackoverflow = -16;
        public const int e_stackunderflow = -17;
        public const int e_syntaxerror = -18;
        public const int e_timeout = -19;
        public const int e_typecheck = -20;
        public const int e_undefined = -21;
        public const int e_undefinedfilename = -22;
        public const int e_undefinedresult = -23;
        public const int e_unmatchedmark = -24;
        public const int e_VMerror = -25;		/* must be the last Level 1 error */

        public static string[] LEVEL1_ERROR_NAMES = {
                 "unknownerror", "dictfull", "dictstackoverflow", "dictstackunderflow",
                 "execstackoverflow", "interrupt", "invalidaccess", "invalidexit",
                 "invalidfileaccess", "invalidfont", "invalidrestore", "ioerror",
                 "limitcheck", "nocurrentpoint", "rangecheck", "stackoverflow",
                 "stackunderflow", "syntaxerror", "timeout", "typecheck", "undefined",
                 "undefinedfilename", "undefinedresult", "unmatchedmark", "VMerror" };


        // Additional Level 2 errors (also in DPS)

        public const int e_configurationerror = -26;
        public const int e_undefinedresource = -27;
        public const int e_unregistered = -28;

        public static string[] LEVEL2_ERROR_NAMES = { "configurationerror", "undefinedresource", "unregistered" };

        // Additional DPS errors

        public const int e_invalidcontext = -29;

        // invalidid is for the NeXT DPS extension
        
        public const int e_invalidid = -30;

        public static string[] DPS_ERROR_NAMES = { "invalidcontext", "invalidid" };

        public static List<string> ERROR_NAMES = new List<string>();


        // Pseudo-errors used internally

        /// <summary>
        /// Internal code for a fatal error.
        /// gs_interpret also returns this for a .quit with a positive exit code.
        /// </summary>
        public const int e_Fatal = -100;

        /// <summary>
        /// Internal code for the .quit operator.
        /// The real quit code is an integer on the operand stack.
        /// gs_interpret returns this only for a .quit with a zero exit code.
        /// </summary>
        public const int e_Quit = -101;

        /// <summary>
        /// Internal code for a normal exit from the interpreter.
        /// Do not use outside of interp.c.
        /// </summary>
        public const int e_InterpreterExit = -102;

        /// <summary>
        /// Internal code that indicates that a procedure has been stored in the
        /// remap_proc of the graphics state, and should be called before retrying
        /// the current token.  This is used for color remapping involving a call
        /// back into the interpreter -- inelegant, but effective.
        /// </summary>
        public const int e_RemapColor = -103;

        /// <summary>
        /// Internal code to indicate we have underflowed the top block
        /// of the e-stack.
        /// </summary>
        public const int e_ExecStackUnderflow = -104;

        /// <summary>
        /// Internal code for the vmreclaim operator with a positive operand.
        /// We need to handle this as an error because otherwise the interpreter
        /// won't reload enough of its state when the operator returns.
        /// </summary>
        public const int e_VMreclaim = -105;

        /// <summary>
        /// Internal code for requesting more input from run_string.
        /// </summary>
        public const int e_NeedInput = -106;

        /// <summary>
        /// Internal code for a normal exit when usage info is displayed.
        /// This allows Window versions of Ghostscript to pause until
        /// the message can be read.
        /// </summary>
        public const int e_Info = -110;

    }
}
