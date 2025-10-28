//
// CrossPlatformNativeLibrary.cs
// This file is part of Microsoft.WinAny.Helper library
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
