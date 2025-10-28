// Copyright (C) 2025 Artifex Software, Inc.
//
// This file is part of Ghostscript.NET.
//
// Ghostscript.NET is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// Ghostscript.NET is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Ghostscript.NET. If not, see 
// <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.

using System;
using System.Runtime.InteropServices;
using Microsoft.WinAny;

namespace Microsoft.WinAny.Interop
{
    /// <summary>
    /// Cross-platform class that helps you to load and use native/unmanaged dynamic-link libraries.
    /// It provides ability to load libraries from the memory (Windows only) or disk.
    /// It's compatible with x86 and x64 libraries on Windows and Linux.
    /// </summary>
    public class CrossPlatformNativeLibrary : IDisposable
    {
        #region Private variables

        private IntPtr _loadedModuleHandle;
        private bool _loadedFromMemory;
        private bool _disposed = false;
        private bool _isWindows;

        #endregion

        #region Constructor - fileName

        /// <summary>
        /// Initializes a new instance of the CrossPlatformNativeLibrary class from a native module stored on disk.
        /// </summary>
        /// <param name="lpLibFileName">Native module file name.</param>
        public CrossPlatformNativeLibrary(string lpLibFileName)
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            if (_isWindows)
            {
                _loadedModuleHandle = WinBase.LoadLibrary(lpLibFileName);
            }
            else
            {
                _loadedModuleHandle = LinuxBase.dlopen(lpLibFileName, LinuxBase.DEFAULT_FLAGS);
            }

            if (_loadedModuleHandle == IntPtr.Zero)
            {
                string errorMessage = _isWindows ? 
                    "Module could not be loaded." : 
                    GetDlerrorMessage();
                throw new Exception(errorMessage);
            }

            _loadedFromMemory = false;
        }

        #endregion

        #region Constructor - buffer (Windows only)

        /// <summary>
        /// Initializes a new instance of the CrossPlatformNativeLibrary class from a native module byte array.
        /// This constructor only works on Windows.
        /// </summary>
        /// <param name="buffer">Native module byte array.</param>
        public CrossPlatformNativeLibrary(byte[] buffer)
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            if (!_isWindows)
            {
                throw new NotSupportedException("Loading libraries from memory is only supported on Windows.");
            }

            // For Windows, we'll need to create a wrapper around DynamicNativeLibrary
            // For now, we'll throw an exception as this is complex to implement
            throw new NotSupportedException("Memory loading is not yet implemented in CrossPlatformNativeLibrary. Use DynamicNativeLibrary directly for Windows memory loading.");
        }

        #endregion

        #region Destructor

        ~CrossPlatformNativeLibrary()
        {
            Dispose(false);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free managed resources
                }

                if (_loadedModuleHandle != IntPtr.Zero)
                {
                    if (_isWindows)
                    {
                        WinBase.FreeLibrary(_loadedModuleHandle);
                    }
                    else
                    {
                        LinuxBase.dlclose(_loadedModuleHandle);
                    }

                    _loadedModuleHandle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        #endregion

        #region GetDelegateForFunction

        /// <summary>
        /// Gets a delegate for the specified function in the loaded library.
        /// </summary>
        /// <param name="functionName">Function name.</param>
        /// <param name="delegateType">The type of the delegate to be returned.</param>
        /// <returns>Delegate instance.</returns>
        public Delegate GetDelegateForFunction(string functionName, Type delegateType)
        {
            IntPtr functionPtr = GetProcAddress(functionName);
            
            if (functionPtr == IntPtr.Zero)
            {
                return null;
            }

            return Marshal.GetDelegateForFunctionPointer(functionPtr, delegateType);
        }

        /// <summary>
        /// Gets a delegate for the specified function in the loaded library.
        /// </summary>
        /// <typeparam name="T">Delegate type.</typeparam>
        /// <param name="functionName">Function name.</param>
        /// <returns>Delegate instance.</returns>
        public T GetDelegateForFunction<T>(string functionName) where T : class
        {
            return (T)(object)GetDelegateForFunction(functionName, typeof(T));
        }

        #endregion

        #region GetProcAddress

        /// <summary>
        /// Gets the address of the specified function in the loaded library.
        /// </summary>
        /// <param name="functionName">Function name.</param>
        /// <returns>Function address.</returns>
        public IntPtr GetProcAddress(string functionName)
        {
            if (_isWindows)
            {
                return WinBase.GetProcAddress(_loadedModuleHandle, functionName);
            }
            else
            {
                return LinuxBase.dlsym(_loadedModuleHandle, functionName);
            }
        }

        #endregion

        #region GetModuleHandle

        /// <summary>
        /// Gets the module handle for internal use.
        /// </summary>
        /// <returns>Module handle.</returns>
        internal IntPtr GetModuleHandle()
        {
            return _loadedModuleHandle;
        }

        #endregion

        #region GetDlerrorMessage

        /// <summary>
        /// Gets the last error message from dlerror() on Linux.
        /// </summary>
        /// <returns>Error message.</returns>
        private string GetDlerrorMessage()
        {
            IntPtr errorPtr = LinuxBase.dlerror();
            if (errorPtr != IntPtr.Zero)
            {
                return Marshal.PtrToStringAnsi(errorPtr);
            }
            return "Unknown error occurred while loading library.";
        }

        #endregion
    }
}
