//
// GhostscriptLibrary.cs
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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WinAny.Interop;

namespace Ghostscript.NET
{
    /// <summary>
    /// Represents a native Ghostscript library.
    /// </summary>
    public class GhostscriptLibrary : IDisposable
    {

        #region Private variables

        private bool _disposed = false;
        private DynamicNativeLibrary _library;
        private CrossPlatformNativeLibrary _crossPlatformLibrary;
        private GhostscriptVersionInfo _version;
        private bool _loadedFromMemory = false;
        private int _revision;
        private bool _isWindows;

        #endregion

        #region Constructor - buffer

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptLibrary class
        /// from the native library represented as the memory buffer.
        /// </summary>
        /// <param name="library">Memory buffer representing native Ghostscript library.</param>
        public GhostscriptLibrary(byte[] library)
        {
            if (library == null)
            {
                throw new ArgumentNullException("library");
            }

            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _loadedFromMemory = true;

            if (_isWindows)
            {
                // check if library is compatibile with a running process
                if (Environment.Is64BitProcess != NativeLibraryHelper.Is64BitLibrary(library))
                {
                    // throw friendly gsdll incompatibility message
                    this.ThrowIncompatibileNativeGhostscriptLibraryException();
                }

                // create DynamicNativeLibrary instance from the memory buffer
                _library = new DynamicNativeLibrary(library);
            }
            else
            {
                throw new NotSupportedException("Loading libraries from memory is only supported on Windows.");
            }

            // get and map native library symbols
            this.Initialize();
        }

        #endregion

        #region Constructor - version

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptLibrary class
        /// from the GhostscriptVersionInfo object.
        /// </summary>
        /// <param name="version">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        public GhostscriptLibrary(GhostscriptVersionInfo version) : this(version, false)
        { }

        #endregion

        #region Constructor - version, fromMemory

        /// <summary>
        /// Initializes a new instance of the Ghostscript.NET.GhostscriptLibrary class
        /// from the GhostscriptVersionInfo object.
        /// </summary>
        /// <param name="version">GhostscriptVersionInfo instance that tells which Ghostscript library to use.</param>
        /// <param name="fromMemory">Tells if the Ghostscript should be loaded from the memory or directly from the disk.</param>
        public GhostscriptLibrary(GhostscriptVersionInfo version, bool fromMemory)
        {
            // check if Ghostscript version is specified
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            // check if specified Ghostscript native library exist on the disk
            if (!File.Exists(version.DllPath))
            {
                throw new DllNotFoundException("Ghostscript native library could not be found.");
            }

            _version = version;
            _loadedFromMemory = fromMemory;
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            // check if library is compatibile with a running process
            if (!CrossPlatformNativeLibraryHelper.IsLibraryCompatible(version.DllPath))
            {
                // throw friendly library incompatibility message
                this.ThrowIncompatibileNativeGhostscriptLibraryException();
            }

            // check wether we need to load Ghostscript native library from the memory or a disk
            if (fromMemory)
            {
                if (_isWindows)
                {
                    // load native Ghostscript library into the memory (Windows only)
                    byte[] buffer = File.ReadAllBytes(version.DllPath);
                    _library = new DynamicNativeLibrary(buffer);
                }
                else
                {
                    throw new NotSupportedException("Loading libraries from memory is only supported on Windows.");
                }
            }
            else
            {
                if (_isWindows)
                {
                    // create DynamicNativeLibrary instance from the local disk file (Windows)
                    _library = new DynamicNativeLibrary(version.DllPath);
                }
                else
                {
                    // create CrossPlatformNativeLibrary instance for Linux
                    _crossPlatformLibrary = new CrossPlatformNativeLibrary(version.DllPath);
                }
            }

            // get and map native library symbols
            this.Initialize();
        }

        #endregion

        #region Destructor

        ~GhostscriptLibrary()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        #region Dispose

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptLibrary instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Dispose - disposing

        /// <summary>
        /// Releases all resources used by the Ghostscript.NET.GhostscriptLibrary instance.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_library != null)
                    {
                        _library.Dispose();
                        _library = null;
                    }

                    if (_crossPlatformLibrary != null)
                    {
                        _crossPlatformLibrary.Dispose();
                        _crossPlatformLibrary = null;
                    }
                }

                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Ghostscript functions

        public gsapi_revision @gsapi_revision = null;
        public gsapi_new_instance @gsapi_new_instance = null;
        public gsapi_delete_instance @gsapi_delete_instance = null;
        public gsapi_set_stdio @gsapi_set_stdio = null;
        public gsapi_set_poll @gsapi_set_poll = null;
        public gsapi_set_display_callback @gsapi_set_display_callback = null;
        public gsapi_set_arg_encoding @gsapi_set_arg_encoding = null;
        public gsapi_init_with_args @gsapi_init_with_args = null;
        public gsapi_run_string_begin @gsapi_run_string_begin = null;
        public gsapi_run_string_continue @gsapi_run_string_continue = null;
        public gsapi_run_string_end @gsapi_run_string_end = null;
        public gsapi_run_string_with_length @gsapi_run_string_with_length = null;
        public gsapi_run_string @gsapi_run_string = null;
        public gsapi_run_ptr_string @gsapi_run_ptr_string = null;
        public gsapi_run_file @gsapi_run_file = null;
        public gsapi_exit @gsapi_exit = null;

        #endregion 

        #region Initialize

        /// <summary>
        /// Get the native library symbols and map them to the appropriate functions/delegates.
        /// </summary>
        private void Initialize()
        {
            if (_isWindows)
            {
                InitializeWindows();
            }
            else
            {
                InitializeLinux();
            }
        }

        /// <summary>
        /// Initialize Windows-specific library symbols.
        /// </summary>
        private void InitializeWindows()
        {
            string symbolMappingError = "Delegate of an exported function couldn't be created for symbol '{0}'";

            this.gsapi_revision = _library.GetDelegateForFunction<gsapi_revision>("gsapi_revision");

            if (this.gsapi_revision == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_revision"));

            gsapi_revision_s rev = new gsapi_revision_s();
            if (this.gsapi_revision(ref rev, System.Runtime.InteropServices.Marshal.SizeOf(rev)) == 0)
            {
                _revision = rev.revision;
            }

            this.gsapi_new_instance = _library.GetDelegateForFunction<gsapi_new_instance>("gsapi_new_instance");

            if (this.gsapi_new_instance == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_new_instance"));

            this.gsapi_delete_instance = _library.GetDelegateForFunction<gsapi_delete_instance>("gsapi_delete_instance");

            if (this.gsapi_delete_instance == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_delete_instance"));

            this.gsapi_set_stdio = _library.GetDelegateForFunction<gsapi_set_stdio>("gsapi_set_stdio");

            if (this.gsapi_set_stdio == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_stdio"));

            this.gsapi_set_poll = _library.GetDelegateForFunction<gsapi_set_poll>("gsapi_set_poll");

            if (this.gsapi_set_poll == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_poll"));

            this.gsapi_set_display_callback = _library.GetDelegateForFunction<gsapi_set_display_callback>("gsapi_set_display_callback");

            if (this.gsapi_set_display_callback == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_display_callback"));

            if (is_gsapi_set_arg_encoding_supported)
            {
                this.gsapi_set_arg_encoding = _library.GetDelegateForFunction<gsapi_set_arg_encoding>("gsapi_set_arg_encoding");

                if (this.gsapi_set_arg_encoding == null)
                    throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_arg_encoding"));
            }

            this.gsapi_init_with_args = _library.GetDelegateForFunction<gsapi_init_with_args>("gsapi_init_with_args");

            if (this.gsapi_init_with_args == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_init_with_args"));

            this.gsapi_run_string_begin = _library.GetDelegateForFunction<gsapi_run_string_begin>("gsapi_run_string_begin");

            if (this.gsapi_run_string_begin == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_begin"));

            this.gsapi_run_string_continue = _library.GetDelegateForFunction<gsapi_run_string_continue>("gsapi_run_string_continue");

            if (this.gsapi_run_string_continue == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_continue"));

            this.gsapi_run_string_end = _library.GetDelegateForFunction<gsapi_run_string_end>("gsapi_run_string_end");

            if (this.gsapi_run_string_end == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_end"));

            this.gsapi_run_string_with_length = _library.GetDelegateForFunction<gsapi_run_string_with_length>("gsapi_run_string_with_length");

            if (this.gsapi_run_string_with_length == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_with_length"));

            this.gsapi_run_string = _library.GetDelegateForFunction<gsapi_run_string>("gsapi_run_string");

            if (this.gsapi_run_string == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string"));

            this.gsapi_run_ptr_string = _library.GetDelegateForFunction<gsapi_run_ptr_string>("gsapi_run_string");

            if (this.gsapi_run_ptr_string == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string"));

            this.gsapi_run_file = _library.GetDelegateForFunction<gsapi_run_file>("gsapi_run_file");

            if (this.gsapi_run_file == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_file"));

            this.gsapi_exit = _library.GetDelegateForFunction<gsapi_exit>("gsapi_exit");

            if (this.gsapi_exit == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_exit"));
        }

        /// <summary>
        /// Initialize Linux-specific library symbols.
        /// </summary>
        private void InitializeLinux()
        {
            string symbolMappingError = "Delegate of an exported function couldn't be created for symbol '{0}'";

            this.gsapi_revision = _crossPlatformLibrary.GetDelegateForFunction<gsapi_revision>("gsapi_revision");

            if (this.gsapi_revision == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_revision"));

            gsapi_revision_s rev = new gsapi_revision_s();
            if (this.gsapi_revision(ref rev, System.Runtime.InteropServices.Marshal.SizeOf(rev)) == 0)
            {
                _revision = rev.revision;
            }

            this.gsapi_new_instance = _crossPlatformLibrary.GetDelegateForFunction<gsapi_new_instance>("gsapi_new_instance");

            if (this.gsapi_new_instance == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_new_instance"));

            this.gsapi_delete_instance = _crossPlatformLibrary.GetDelegateForFunction<gsapi_delete_instance>("gsapi_delete_instance");

            if (this.gsapi_delete_instance == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_delete_instance"));

            this.gsapi_set_stdio = _crossPlatformLibrary.GetDelegateForFunction<gsapi_set_stdio>("gsapi_set_stdio");

            if (this.gsapi_set_stdio == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_stdio"));

            this.gsapi_set_poll = _crossPlatformLibrary.GetDelegateForFunction<gsapi_set_poll>("gsapi_set_poll");

            if (this.gsapi_set_poll == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_poll"));

            this.gsapi_set_display_callback = _crossPlatformLibrary.GetDelegateForFunction<gsapi_set_display_callback>("gsapi_set_display_callback");

            if (this.gsapi_set_display_callback == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_display_callback"));

            if (is_gsapi_set_arg_encoding_supported)
            {
                this.gsapi_set_arg_encoding = _crossPlatformLibrary.GetDelegateForFunction<gsapi_set_arg_encoding>("gsapi_set_arg_encoding");

                if (this.gsapi_set_arg_encoding == null)
                    throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_set_arg_encoding"));
            }

            this.gsapi_init_with_args = _crossPlatformLibrary.GetDelegateForFunction<gsapi_init_with_args>("gsapi_init_with_args");

            if (this.gsapi_init_with_args == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_init_with_args"));

            this.gsapi_run_string_begin = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_string_begin>("gsapi_run_string_begin");

            if (this.gsapi_run_string_begin == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_begin"));

            this.gsapi_run_string_continue = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_string_continue>("gsapi_run_string_continue");

            if (this.gsapi_run_string_continue == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_continue"));

            this.gsapi_run_string_end = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_string_end>("gsapi_run_string_end");

            if (this.gsapi_run_string_end == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_end"));

            this.gsapi_run_string_with_length = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_string_with_length>("gsapi_run_string_with_length");

            if (this.gsapi_run_string_with_length == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string_with_length"));

            this.gsapi_run_string = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_string>("gsapi_run_string");

            if (this.gsapi_run_string == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string"));

            this.gsapi_run_ptr_string = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_ptr_string>("gsapi_run_string");

            if (this.gsapi_run_ptr_string == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_string"));

            this.gsapi_run_file = _crossPlatformLibrary.GetDelegateForFunction<gsapi_run_file>("gsapi_run_file");

            if (this.gsapi_run_file == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_run_file"));

            this.gsapi_exit = _crossPlatformLibrary.GetDelegateForFunction<gsapi_exit>("gsapi_exit");

            if (this.gsapi_exit == null)
                throw new GhostscriptException(string.Format(symbolMappingError, "gsapi_exit"));
        }

        #endregion

        #region ThrowIncompatibileNativeGhostscriptLibraryException

        /// <summary>
        /// Throws friendly library incompatibility message.
        /// </summary>
        private void ThrowIncompatibileNativeGhostscriptLibraryException()
        {
            string errorMessage = CrossPlatformNativeLibraryHelper.GetCompatibilityErrorMessage();
            throw new BadImageFormatException(errorMessage);
        }

        #endregion

        #region Revision

        public int Revision
        {
            get { return _revision; }
        }

        #endregion

        #region is_gsapi_set_arg_encoding

        public bool is_gsapi_set_arg_encoding_supported
        {
            get
            {
                if (_revision >= 910)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

    }
}
