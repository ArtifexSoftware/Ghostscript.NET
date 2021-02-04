//
// iapi.h.cs
// This file is part of Ghostscript.NET library
//
// Author: Josip Habjan (habjan@gmail.com, http://www.linkedin.com/in/habjan) 
// Copyright (c) 2013-2016 by Josip Habjan. All rights reserved.
//
// Author ported parts of this code from AFPL Ghostscript. 
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
using System.Runtime.InteropServices;

namespace Ghostscript.NET
{

    #region gsapi_revision_s

    [StructLayout(LayoutKind.Sequential)]
    public struct gsapi_revision_s
    {
        public IntPtr product;
        public IntPtr copyright;
        public int revision;
        public int revisiondate;
    }

    #endregion

    /// <summary>
    /// Get version numbers and strings.
    /// This is safe to call at any time.
    /// You should call this first to make sure that the correct version
    /// of the Ghostscript is being used.
    /// pr is a pointer to a revision structure.
    /// len is the size of this structure in bytes.
    /// Returns 0 if OK, or if len too small (additional parameters
    /// have been added to the structure) it will return the required
    /// size of the structure.
    /// </summary>
    /// <param name="pr"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_revision(ref gsapi_revision_s pr, Int32 len);

    /// <summary>
    /// Create a new instance of Ghostscript.
    /// This instance is passed to most other API functions.
    /// The caller_handle will be provided to callback functions.
    /// </summary>
    /// <param name="pinstance"></param>
    /// <param name="caller_handle"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

    /// <summary>
    /// Destroy an instance of Ghostscript
    /// Before you call this, Ghostscript must have finished.
    /// If Ghostscript has been initialised, you must call gsapi_exit()
    /// before gsapi_delete_instance.
    /// </summary>
    /// <param name="instance"></param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void gsapi_delete_instance(IntPtr instance);

    /// <summary>
    /// Set the callback functions for stdio
    /// The stdin callback function should return the number of
    /// characters read, 0 for EOF, or -1 for error.
    /// The stdout and stderr callback functions should return
    /// the number of characters written.
    /// If a callback address is NULL, the real stdio will be used.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="stdin_fn"></param>
    /// <param name="stdout_fn"></param>
    /// <param name="stderr_fn"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_set_stdio(IntPtr instance, gsapi_stdio_callback stdin_fn, gsapi_stdio_callback stdout_fn, gsapi_stdio_callback stderr_fn);

    /// <summary>
    /// Set the callback function for polling.
    /// This is used for handling window events or cooperative
    /// multitasking.  This function will only be called if
    /// Ghostscript was compiled with CHECK_INTERRUPTS
    /// as described in gpcheck.h.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="poll_fn"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_set_poll(IntPtr instance, gsapi_pool_callback poll_fn);

    /// <summary>
    /// Set the display device callback structure.
    /// If the display device is used, this must be called
    /// after gsapi_new_instance() and before gsapi_init_with_args().
    /// See gdevdisp.h for more details.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_set_display_callback(IntPtr instance, IntPtr callback);

    /// <summary>
    /// Set the encoding used for the args. By default we assume
    /// 'local' encoding. For windows this equates to whatever the current
    /// codepage is. For linux this is utf8.
    /// 
    /// Use of this API (gsapi) with 'local' encodings (and hence without calling
    /// this function) is now deprecated!
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_set_arg_encoding(IntPtr instance, GS_ARG_ENCODING encoding);

    public enum GS_ARG_ENCODING : int
    {
        LOCAL = 0,
        UTF8 = 1,
        UTF16LE = 2
    }

    /// <summary>
    /// Initialise the interpreter.
    /// This calls gs_main_init_with_args() in imainarg.c
    /// 1. If quit or EOF occur during gsapi_init_with_args(), 
    ///    the return value will be e_Quit.  This is not an error. 
    ///    You must call gsapi_exit() and must not call any other
    ///    gsapi_XXX functions.
    /// 2. If usage info should be displayed, the return value will be e_Info
    ///    which is not an error.  Do not call gsapi_exit().
    /// 3. Under normal conditions this returns 0.  You would then 
    ///    call one or more gsapi_run_*() functions and then finish
    ///    with gsapi_exit().
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="argc"></param>
    /// <param name="argv"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet=CharSet.Ansi)]
    public delegate int gsapi_init_with_args(IntPtr instance, Int32 argc, string[] argv);

    // The gsapi_run_* functions are like gs_main_run_* except
    // that the error_object is omitted.
    // If these functions return <= -100, either quit or a fatal
    // error has occured.  You then call gsapi_exit() next.
    // The only exception is gsapi_run_string_continue()
    // which will return e_NeedInput if all is well.

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_run_string_begin(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet=CharSet.Ansi)]
    public delegate int gsapi_run_string_continue(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_run_string_end(IntPtr instance, Int32 user_errors, out Int32 pexit_code);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet=CharSet.Ansi)]
    public delegate int gsapi_run_string_with_length(IntPtr instance, String str, UInt32 length, Int32 user_errors, out Int32 pexit_code);
    
    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet=CharSet.Ansi)]
    public delegate int gsapi_run_string(IntPtr instance, String str, Int32 user_errors, out Int32 pexit_code);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    public delegate int gsapi_run_ptr_string(IntPtr instance, IntPtr str, Int32 user_errors, out Int32 pexit_code);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet=CharSet.Ansi)]
    public delegate int gsapi_run_file(IntPtr instance, String file_name, Int32 user_errors, out Int32 pexit_code);

    /// <summary>
    /// Exit the interpreter.
    /// This must be called on shutdown if gsapi_init_with_args()
    /// has been called, and just before gsapi_delete_instance().
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate int gsapi_exit(IntPtr instance);

}
