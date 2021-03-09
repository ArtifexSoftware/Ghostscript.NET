//
// GhostscriptInterpreter.cs
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
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace Ghostscript.NET.Interpreter
{
    /// <summary>
    /// Represents a Ghostscript interpreter.
    /// </summary>
    public class GhostscriptInterpreter : IDisposable
    {

        #region Private constants

        private const int RUN_STRING_MAX_LENGTH = 65535;

        #endregion

        #region Private variables

        private bool _disposed = false;
        private GhostscriptLibrary _gs = null;
        private IntPtr _gs_instance = IntPtr.Zero;
        private GhostscriptStdIO _stdIO = null;
        private GhostscriptDisplayDeviceHandler _displayDevice = null;
        private IntPtr _displayDevice_callback_handle = IntPtr.Zero;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptInterpreter class.
        /// </summary>
        public GhostscriptInterpreter()
            : this(GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL), false)
        { }

        #endregion

        #region Constructor - library

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptInterpreter class.
        /// </summary>
        /// <param name="library">Memory buffer representing native Ghostscript library.</param>
        public GhostscriptInterpreter(byte[] library)
        {
            if (library == null)
            {
                throw new ArgumentNullException("library");
            }

            // load ghostscript native library
            _gs = new GhostscriptLibrary(library);

            // initialize Ghostscript interpreter
            this.Initialize();
        }

        #endregion

        #region Constructor - version

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptInterpreter class.
        /// </summary>
        /// <param name="version">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        public GhostscriptInterpreter(GhostscriptVersionInfo version) : this(version, false)
        { }

        #endregion

        #region Constructor - version, fromMemory

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptInterpreter class.
        /// </summary>
        /// <param name="version">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <param name="fromMemory">Tells if the Ghostscript should be loaded from the memory or directly from the disk.</param>
        public GhostscriptInterpreter(GhostscriptVersionInfo version, bool fromMemory)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            // load ghostscript native library
            _gs = new GhostscriptLibrary(version, fromMemory);

            // initialize Ghostscript interpreter
            this.Initialize();
        }

        #endregion

        #region Destructor

        ~GhostscriptInterpreter()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        #region Dispose

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptInterpreter instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptInterpreter instance.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // GSAPI: exit the interpreter
                    _gs.gsapi_exit(_gs_instance);

                    // GSAPI: destroy an instance of Ghostscript
                    _gs.gsapi_delete_instance(_gs_instance);

                    // release all resource used by Ghostscript library
                    _gs.Dispose();
                }

                // check if the display device callback handler is attached
                if (_displayDevice_callback_handle != IntPtr.Zero)
                {
                    // free earlier allocated memory used for the display device callback
                    Marshal.FreeCoTaskMem(_displayDevice_callback_handle);
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of Ghostscript interpreter.
        /// </summary>
        private void Initialize()
        {
            // GSAPI: create a new instance of Ghostscript
            int rc_ins = _gs.gsapi_new_instance(out _gs_instance, IntPtr.Zero);

            if (ierrors.IsError(rc_ins))
            {
                throw new GhostscriptAPICallException("gsapi_new_instance", rc_ins);
            }
        }

        #endregion

        #region Setup

        /// <summary>
        /// Sets the stdio and display device callback handlers.
        /// </summary>
        /// <param name="stdIO">Stdio callback handler.</param>
        /// <param name="displayDevice">DisplayDevice callback handler.</param>
        public void Setup(GhostscriptStdIO stdIO, GhostscriptDisplayDeviceHandler displayDevice)
        {
            // check if we need to set stdio handler
            if (stdIO != null)
            {
                // check if stdio handler is not already set
                if (_stdIO == null)
                {
                    // GSAPI: set the stdio callback handlers
                    int rc_stdio = _gs.gsapi_set_stdio(_gs_instance,
                                            stdIO != null ? stdIO._std_in : null,
                                            stdIO != null ? stdIO._std_out : null,
                                            stdIO != null ? stdIO._std_err : null);

                    // check if the stdio callback handlers are set correctly
                    if (ierrors.IsError(rc_stdio))
                    {
                        throw new GhostscriptAPICallException("gsapi_set_stdio", rc_stdio);
                    }

                    // remember it
                    _stdIO = stdIO;
                }
                else
                {
                    throw new GhostscriptException("StdIO callback handler is already set.");
                }
            }

            // check if a custom display device needs to be used
            if (displayDevice != null)
            {
                // check if display device is already set
                if (_displayDevice == null)
                {
                    // allocate a memory for the display device callback handler
                    _displayDevice_callback_handle = Marshal.AllocCoTaskMem(displayDevice._callback.size);

                    // copy display device callback structure content to the pre-allocated block of memory
                    Marshal.StructureToPtr(displayDevice._callback, _displayDevice_callback_handle, true);

                    // GSAPI: set the display device callback handler
                    int rc_dev = _gs.gsapi_set_display_callback(_gs_instance, _displayDevice_callback_handle);

                    // check if the display callback handler is set correctly
                    if (ierrors.IsError(rc_dev))
                    {
                        throw new GhostscriptAPICallException("gsapi_set_display_callback", rc_dev);
                    }

                    // remember it
                    _displayDevice = displayDevice;

                }
                else
                {
                    throw new GhostscriptException("DisplayDevice callback is already set!");
                }
            }

        }

        #endregion

        #region InitArgs

        /// <summary>
        /// Initializes the interpreter.
        /// </summary>
        public void InitArgs(string[] args)
        {
            if (_gs.is_gsapi_set_arg_encoding_supported)
            {
                // set the encoding to UTF8
                int rc_enc = _gs.gsapi_set_arg_encoding(_gs_instance, GS_ARG_ENCODING.UTF8);
            }

            string[] utf8args = new string[args.Length];

            for(int i = 0; i < args.Length; i++)
            {
                utf8args[i] = StringHelper.ToUtf8String(args[i]);
            }
            
            // GSAPI: initialize the interpreter
            int rc_init = _gs.gsapi_init_with_args(_gs_instance, utf8args.Length, utf8args);

            // check if the interpreter is initialized correctly
            if (ierrors.IsError(rc_init))
            {
                throw new GhostscriptAPICallException("gsapi_init_with_args", rc_init);
            }
        }

        #endregion

        #region Run

        /// <summary>
        /// Runs a string.
        /// </summary>
        public int Run(string str)
        {
            lock (this)
            {
                int exit_code;

                // check if the string we are trying to run doesn't exceed max length for the 'run_string' function
                if (str.Length < RUN_STRING_MAX_LENGTH)
                {
                    // GSAPI: run the string
                    int rc_run = _gs.gsapi_run_string(_gs_instance, str, 0, out exit_code);

                    if (ierrors.IsFatalIgnoreNeedInput(rc_run))
                    {
                        throw new GhostscriptAPICallException("gsapi_run_string", rc_run);
                    }

                    return rc_run;
                }
                else // we need to split a string into chunks
                {
                    // GSAPI: prepare a Ghostscript for running string in chunks
                    int rc_run_beg = _gs.gsapi_run_string_begin(_gs_instance, 0, out exit_code);

                    if (ierrors.IsFatalIgnoreNeedInput(rc_run_beg))
                    {
                        throw new GhostscriptAPICallException("gsapi_run_string_begin", rc_run_beg);
                    }

                    int chunkStart = 0;

                    // start splitting a string into chunks
                    for (int size = str.Length; size > 0; size -= RUN_STRING_MAX_LENGTH)
                    {
                        int chunkSize = (size < RUN_STRING_MAX_LENGTH) ? size : RUN_STRING_MAX_LENGTH;
                        string chunk = str.Substring(chunkStart, chunkSize);

                        // GSAPI: run a chunk
                        int rc_run_con = _gs.gsapi_run_string_continue(_gs_instance, chunk, (uint)chunkSize, 0, out exit_code);

                        if (ierrors.IsFatalIgnoreNeedInput(rc_run_con))
                        {
                            throw new GhostscriptAPICallException("gsapi_run_string_continue", rc_run_con);
                        }

                        chunkStart += chunkSize;
                    }

                    // GSAPI: notify Ghostscript we are done with running chunked string
                    int rc_run_end = _gs.gsapi_run_string_end(_gs_instance, 0, out exit_code);

                    if (ierrors.IsFatalIgnoreNeedInput(rc_run_end))
                    {
                        throw new GhostscriptAPICallException("gsapi_run_string_end", rc_run_end);
                    }

                    return rc_run_end;
                }
            }
        }

        #endregion

        #region Run

        /// <summary>
        /// Runs a string.
        /// </summary>
        internal int Run(IntPtr str)
        {
            lock (this)
            {
                int exit_code;

                int rc_run = _gs.gsapi_run_ptr_string(_gs_instance, str, 0, out exit_code);

                if (ierrors.IsFatalIgnoreNeedInput(rc_run))
                {
                    throw new GhostscriptAPICallException("gsapi_run_string", rc_run);
                }

                return rc_run;
            }
        }

        #endregion

        #region RunFile

        /// <summary>
        /// Runs a PostScript file.
        /// </summary>
        public void RunFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Couldn't find input file.", path);
            }

            int exit_code;

            // GSAPI: tell a Ghostscript to run a file
            int rc_run = _gs.gsapi_run_file(_gs_instance, path, 0, out exit_code);

            if (ierrors.IsFatal(rc_run))
            {
                throw new GhostscriptAPICallException("gsapi_run_file", rc_run);
            }
        }

        #endregion

        #region LibraryRevision

        public int LibraryRevision
        {
            get { return _gs.Revision; }
        }

        #endregion

        #region GhostscriptLibrary

        public GhostscriptLibrary GhostscriptLibrary
        {
            get { return _gs; }
        }

        #endregion

    }
}
